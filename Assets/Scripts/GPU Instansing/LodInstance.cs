using System;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LodSystem))]
public class LodInstance : MonoBehaviour
{
    public enum LodLevels
    {
        _ONLY_DRAW_DIST_LIMIT = 0,
        _1_LEVEL,
        _2_LEVELS,
        _3_LEVELS,
    }
    
    [SerializeField]
    string objectRenderPositionTag;

    [SerializeField] LodLevels maxLodLevel;
    
    [SerializeField]
    Material material;
    [FormerlySerializedAs("lodLevel0mesh")] [FormerlySerializedAs("mesh0")] [FormerlySerializedAs("chairSeatMesh0")] [SerializeField]
    Mesh lodLevel0Mesh;
    [FormerlySerializedAs("mesh1")] [FormerlySerializedAs("chairSeatMesh1")] [SerializeField]
    Mesh lodLevel1Mesh;
    [FormerlySerializedAs("mesh2")] [FormerlySerializedAs("chairSeatMesh2")] [SerializeField]
    Mesh lodLevel2Mesh;
    [FormerlySerializedAs("mesh3")] [FormerlySerializedAs("chairSeatMesh3")] [SerializeField]
    Mesh lodLevel3Mesh;
    [SerializeField]
    float lod1Distance;
    [SerializeField]
    float lod2Distance;
    [SerializeField]
    float lod3Distance;
    [SerializeField]
    float maxDrawDistance;
    [SerializeField] 
    float ignoreFovDistance;

    LodSystem lod_system;
    
    ComputeBuffer initial_positions_buffer;
    ComputeBuffer[] lod_positions_buffer;
    ComputeBuffer dispatch_buffer;
    CommandBuffer command_buffer;
    GraphicsBuffer[] arguments_buffer;
    
    ComputeShader compute_shader;
    
    RenderParams[] render_params;

    int kernel_index;
    uint batch_size;
    float render_angle;
    Transform[] render_point_transforms;
    
    public void init(LodSystem s)
    {
        lod_system = s;

        kernel_index = lod_system.kernel_index;
        batch_size = lod_system.batch_size;
        render_angle = lod_system.renderAngle;

        init_data();
    }
    
    void OnDestroy()
    {
        initial_positions_buffer?.Release();
        lod_positions_buffer?.for_each(x=>x?.Release());
        dispatch_buffer?.Release();
        command_buffer?.Release();
        arguments_buffer?.for_each(x=>x?.Release());
    }

    void init_data()
    {
        var matrices = init_trs_matrices();

        var matrices_count = matrices.Length;
        
        init_argument_buffers();
        
        init_dispatch_buffer(matrices_count);

        init_positions_buffer(matrices);

        init_lod_buffers(matrices_count);

        init_compute_shader(matrices_count);

        init_command_buffer();
        
        init_rendering_params();
    }

    Matrix4x4[] init_trs_matrices()
    {
        var objects = GameObject.FindObjectsByType<LodObjectPosition>(FindObjectsInactive.Include, FindObjectsSortMode.None);// GetComponentsInChildren(type, true);
        var length = objects.Length;
        var TRS_matrices = new Matrix4x4[length];

        for (var i = 0; i < length; i++)
        {
            if(objects[i].tag != objectRenderPositionTag) continue;
            
            var obj = objects[i].transform;
            TRS_matrices[i] = Matrix4x4.TRS(obj.position, obj.rotation, obj.localScale);
        }
        
        //objects = new LodObjectPosition[30000];
        //length = objects.Length;
        //TRS_matrices = new Matrix4x4[length];
        //for (var i = 0; i < length; i++)
        //{
        //    TRS_matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 100, Quaternion.identity, Vector3.one);
        //}

        return TRS_matrices;
    }

    void init_lod_buffers(int max_objects_count)
    {
        lod_positions_buffer = new ComputeBuffer[4];
        
        lod_positions_buffer[0] = new ComputeBuffer(max_objects_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
        lod_positions_buffer[1] = new ComputeBuffer(max_objects_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
        lod_positions_buffer[2] = new ComputeBuffer(max_objects_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
        lod_positions_buffer[3] = new ComputeBuffer(max_objects_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
    }

    void init_positions_buffer(Matrix4x4[] positions)
    {
        initial_positions_buffer = new ComputeBuffer(positions.Length, sizeof(float) * 4 * 4, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        initial_positions_buffer.SetData(positions);
    }

    void init_dispatch_buffer(int max_objects_count)
    {
        dispatch_buffer = new ComputeBuffer(1, sizeof(int) * 3, ComputeBufferType.IndirectArguments);
        dispatch_buffer.SetData(new [] {Mathf.CeilToInt((float)max_objects_count/batch_size) , 1, 1 });
    }

    void init_compute_shader(int max_objects_count)
    {
        compute_shader = Instantiate(lod_system.computeShader);
        
        compute_shader.SetFloat("fov_dot",Mathf.Cos(render_angle * Mathf.Deg2Rad));
        compute_shader.SetFloat("ignore_fov_distance",ignoreFovDistance*ignoreFovDistance);
        
        var distances_array = new float[4*4];
        distances_array[0] = lod1Distance * lod1Distance;
        distances_array[4] = lod2Distance * lod2Distance;
        distances_array[8] = lod3Distance * lod3Distance;
        distances_array[12] = maxDrawDistance * maxDrawDistance;
        
        compute_shader.SetFloats("lod_distances", distances_array);
        
        compute_shader.SetFloat("model_count", max_objects_count);
        
        compute_shader.SetInt("lod_count", (int)maxLodLevel);
        
        compute_shader.SetBuffer(kernel_index, "model_positions_buffer", initial_positions_buffer);
        
        compute_shader.SetBuffer(kernel_index, "lod_buffers[0]", lod_positions_buffer[0]);
        compute_shader.SetBuffer(kernel_index, "lod_buffers[1]", lod_positions_buffer[1]);
        compute_shader.SetBuffer(kernel_index, "lod_buffers[2]", lod_positions_buffer[2]);
        compute_shader.SetBuffer(kernel_index, "lod_buffers[3]", lod_positions_buffer[3]);
    }

    void init_argument_buffers()
    {
        arguments_buffer = new GraphicsBuffer[4];
        arguments_buffer[0] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        arguments_buffer[1] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        arguments_buffer[2] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        arguments_buffer[3] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        
        var commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        
        commandData[0].indexCountPerInstance = lodLevel0Mesh.GetIndexCount(0);
        arguments_buffer[0].SetData(commandData);
        
        commandData[0].indexCountPerInstance = lodLevel1Mesh.GetIndexCount(0);
        arguments_buffer[1].SetData(commandData);
        
        commandData[0].indexCountPerInstance = lodLevel2Mesh.GetIndexCount(0);
        arguments_buffer[2].SetData(commandData);
        
        commandData[0].indexCountPerInstance = lodLevel3Mesh.GetIndexCount(0);
        arguments_buffer[3].SetData(commandData);
    }
    
    void init_command_buffer()
    {
        command_buffer = new CommandBuffer();
        command_buffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
        
        command_buffer.SetBufferCounterValue(lod_positions_buffer[0],0);
        if((int)maxLodLevel > (int)LodLevels._ONLY_DRAW_DIST_LIMIT)
            command_buffer.SetBufferCounterValue(lod_positions_buffer[1],0);
        if((int)maxLodLevel > (int)LodLevels._1_LEVEL)
            command_buffer.SetBufferCounterValue(lod_positions_buffer[2],0);
        if((int)maxLodLevel > (int)LodLevels._2_LEVELS)
            command_buffer.SetBufferCounterValue(lod_positions_buffer[3],0);
        
        command_buffer.DispatchCompute(compute_shader, kernel_index, dispatch_buffer, 0);
        
        command_buffer.CopyCounterValue(lod_positions_buffer[0],arguments_buffer[0],sizeof(uint));
        if(maxLodLevel == LodLevels._ONLY_DRAW_DIST_LIMIT) return;
        command_buffer.CopyCounterValue(lod_positions_buffer[1],arguments_buffer[1],sizeof(uint));
        if(maxLodLevel == LodLevels._1_LEVEL) return;
        command_buffer.CopyCounterValue(lod_positions_buffer[2],arguments_buffer[2],sizeof(uint));
        if(maxLodLevel == LodLevels._2_LEVELS) return;
        command_buffer.CopyCounterValue(lod_positions_buffer[3],arguments_buffer[3],sizeof(uint));
    }
    
    void init_rendering_params()
    {
        render_params = new RenderParams[4];
        
        render_params[0] = new RenderParams(material);
        render_params[0].matProps = new MaterialPropertyBlock();
        render_params[0].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params[0].matProps.SetBuffer("_TRS_Array", lod_positions_buffer[0]);
        render_params[0].matProps.SetColor("_Col", Color.blue);
        render_params[0].matProps.SetInt("_Instanced", 1);
        
        render_params[1] = new RenderParams(material);
        render_params[1].matProps = new MaterialPropertyBlock();
        render_params[1].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params[1].matProps.SetBuffer("_TRS_Array", lod_positions_buffer[1]);
        render_params[1].matProps.SetColor("_Col", Color.yellow);
        render_params[1].matProps.SetInt("_Instanced", 1);
        
        render_params[2] = new RenderParams(material);
        render_params[2].matProps = new MaterialPropertyBlock();
        render_params[2].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params[2].matProps.SetBuffer("_TRS_Array", lod_positions_buffer[2]);
        render_params[2].matProps.SetColor("_Col", Color.red);
        render_params[2].matProps.SetInt("_Instanced", 1);
        
        render_params[3] = new RenderParams(material);
        render_params[3].matProps = new MaterialPropertyBlock();
        render_params[3].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params[3].matProps.SetBuffer("_TRS_Array", lod_positions_buffer[3]);
        render_params[3].matProps.SetColor("_Col", Color.magenta);
        render_params[3].matProps.SetInt("_Instanced", 1);
    }
    
    public void render_object()
    {
        Graphics.ExecuteCommandBufferAsync(command_buffer, ComputeQueueType.Background);
        
        Graphics.RenderMeshIndirect(render_params[0], lodLevel0Mesh, arguments_buffer[0]);
        if(maxLodLevel == LodLevels._ONLY_DRAW_DIST_LIMIT) return;
        Graphics.RenderMeshIndirect(render_params[1], lodLevel1Mesh, arguments_buffer[1]);
        if(maxLodLevel == LodLevels._1_LEVEL) return;
        Graphics.RenderMeshIndirect(render_params[2], lodLevel2Mesh, arguments_buffer[2]);
        if(maxLodLevel == LodLevels._2_LEVELS) return;
        Graphics.RenderMeshIndirect(render_params[3], lodLevel3Mesh, arguments_buffer[3]);
    }
}
