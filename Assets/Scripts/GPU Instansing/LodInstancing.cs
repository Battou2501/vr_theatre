using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class LodInstancing : MonoBehaviour
{
    [SerializeField]
    ComputeShader computeShader;
    [SerializeField]
    Material material;
    [SerializeField]
    Mesh chairSeatMesh0;
    [SerializeField]
    Mesh chairSeatMesh1;
    [SerializeField]
    Mesh chairSeatMesh2;
    [SerializeField]
    Mesh chairSeatMesh3;
    [SerializeField]
    Mesh chairSideMesh0;
    [SerializeField]
    Mesh chairSideMesh1;
    [SerializeField]
    Mesh chairSideMesh2;
    [SerializeField]
    Mesh chairSideMesh3;
    [SerializeField]
    float renderAngle;
    [SerializeField]
    float lod1Distance;
    [SerializeField]
    float lod2Distance;
    [SerializeField]
    float lod3Distance;
    [SerializeField]
    float maxDrawDistance;

    //Matrix4x4[] seat_TRS_matrices;
    Matrix4x4[] side_TRS_matrices;
    
    ComputeBuffer seat_positions_buffer;
    ComputeBuffer side_positions_buffer;
    
    ComputeBuffer seat_buffer_lod_0;
    ComputeBuffer seat_buffer_lod_1;
    ComputeBuffer seat_buffer_lod_2;
    ComputeBuffer seat_buffer_lod_3;
    
    ComputeBuffer side_buffer_lod_0;
    ComputeBuffer side_buffer_lod_1;
    ComputeBuffer side_buffer_lod_2;
    ComputeBuffer side_buffer_lod_3;

    ComputeShader compute_shader_seats;
    ComputeShader compute_shader_sides;

    ComputeBuffer dispatch_buffer_seats;
    ComputeBuffer dispatch_buffer_sides;
    
    CommandBuffer command_buffer_seats;
    CommandBuffer command_buffer_sides;
    
    GraphicsBuffer[] arguments_buffer_seats;
    
    GraphicsBuffer arguments_buffer_sides_0;
    GraphicsBuffer arguments_buffer_sides_1;
    GraphicsBuffer arguments_buffer_sides_2;
    GraphicsBuffer arguments_buffer_sides_3;

    RenderParams[] render_params_seats;
    
    RenderParams render_params_sides_0;
    RenderParams render_params_sides_1;
    RenderParams render_params_sides_2;
    RenderParams render_params_sides_3;
    
    Transform main_camera_transform;

    int kernel_index;
    uint batch_size;
    
    void Start()
    {
        main_camera_transform = Camera.main.transform;
        
        kernel_index = computeShader.FindKernel("CSMain");
        computeShader.GetKernelThreadGroupSizes(kernel_index, out batch_size,out _, out _);
        
        init_seats_data();

        //compute_shader_sides = Instantiate(computeShader);
        
        //compute_shader_sides.SetFloat("fov_dot",Mathf.Cos(renderAngle * Mathf.Deg2Rad));
        //compute_shader_sides.SetFloat("lod_1_dist", lod1Distance);
        //compute_shader_sides.SetFloat("lod_2_dist", lod2Distance);
        //compute_shader_sides.SetFloat("lod_3_dist", lod3Distance);
        
        
        
        //var sides = GetComponentsInChildren<ChairSide>(true).ToArray();
        //sides = new ChairSide[100];
        //side_TRS_matrices = new Matrix4x4[sides.Length];

        //compute_shader_sides.SetFloat("model_count", sides.Length);
        
        //for (var i = 0; i < sides.Length; i++)
        //{
        //    var side = sides[i].transform;
        //    side_TRS_matrices[i] = Matrix4x4.TRS(side.position, side.rotation, side.localScale);
        //}
        
        //arguments_buffer_sides = new ComputeBuffer(4, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);
        
        //arguments_buffer_sides_0 = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        //arguments_buffer_sides_1 = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        //arguments_buffer_sides_2 = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        //arguments_buffer_sides_3 = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        
        //commandData[0].indexCountPerInstance = chairSideMesh0.GetIndexCount(0);
        //arguments_buffer_sides_0.SetData(commandData);
        
        //commandData[0].indexCountPerInstance = chairSideMesh1.GetIndexCount(0);
        //arguments_buffer_sides_1.SetData(commandData);
        
        //commandData[0].indexCountPerInstance = chairSideMesh2.GetIndexCount(0);
        //arguments_buffer_sides_2.SetData(commandData);
        
        //commandData[0].indexCountPerInstance = chairSideMesh3.GetIndexCount(0);
        //arguments_buffer_sides_3.SetData(commandData);
        
        //dispatch_buffer_sides = new ComputeBuffer(1, sizeof(uint) * 3, ComputeBufferType.IndirectArguments);
        //dispatch_buffer_sides.SetData(new uint[3] {batch_size , 0, 0 });
        
        //side_positions_buffer = new ComputeBuffer(sides.Length, sizeof(float) * 4 * 4, ComputeBufferType.Structured);
        
        //side_buffer_lod_0 = new ComputeBuffer(sides.Length, sizeof(float)*4*4, ComputeBufferType.Append);
        //side_buffer_lod_1 = new ComputeBuffer(sides.Length, sizeof(float)*4*4, ComputeBufferType.Append);
        //side_buffer_lod_2 = new ComputeBuffer(sides.Length, sizeof(float)*4*4, ComputeBufferType.Append);
        //side_buffer_lod_3 = new ComputeBuffer(sides.Length, sizeof(float)*4*4, ComputeBufferType.Append);
        
        
        //command_buffer_sides = new CommandBuffer();
        //command_buffer_sides.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
        //
        
        //
        //command_buffer_sides.SetBufferCounterValue(side_buffer_lod_0,0);
        //command_buffer_sides.SetBufferCounterValue(side_buffer_lod_1,0);
        //command_buffer_sides.SetBufferCounterValue(side_buffer_lod_2,0);
        //command_buffer_sides.SetBufferCounterValue(side_buffer_lod_3,0);
        //
        
        //command_buffer_sides.DispatchCompute(compute_shader_sides, compute_shader_sides.FindKernel("CSMain"), dispatch_buffer_sides, 0);
        //
        
        //
        //command_buffer_sides.CopyCounterValue(side_buffer_lod_0,arguments_buffer_sides_0,sizeof(uint));
        //command_buffer_sides.CopyCounterValue(side_buffer_lod_1,arguments_buffer_sides_1,sizeof(uint));
        //command_buffer_sides.CopyCounterValue(side_buffer_lod_2,arguments_buffer_sides_2,sizeof(uint));
        //command_buffer_sides.CopyCounterValue(side_buffer_lod_3,arguments_buffer_sides_3,sizeof(uint));
        
    }

    void init_seats_data()
    {
        var seats = init_trs_matrices_seats();

        var seats_count = seats.Length;
        
        init_argument_buffers_seats();
        
        init_dispatch_buffer_seats(seats_count);

        init_positions_buffer_seats(seats);

        init_lod_buffers_seats(seats_count);

        init_compute_shader_seats(seats_count);

        init_seats_command_buffer();
        
        init_seats_rendering_params();
    }

    Matrix4x4[] init_trs_matrices_seats()
    {
        var seats = GetComponentsInChildren<ChairSeat>(true).ToArray();
        seats = new ChairSeat[30000];
        //seat_TRS_matrices = new Matrix4x4[seats.Length];

        //for (var i = 0; i < seats.Length; i++)
        //{
        //    var seat = seats[i].transform;
        //    seat_TRS_matrices[i] = Matrix4x4.TRS(seat.position, seat.rotation, seat.localScale);
        //}
        
        var seat_TRS_matrices = new Matrix4x4[seats.Length];
        for (var i = 0; i < seats.Length; i++)
        {
            seat_TRS_matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 100, Quaternion.identity, Vector3.one);
        }

        return seat_TRS_matrices;
    }

    void init_lod_buffers_seats(int seats_count)
    {
        seat_buffer_lod_0 = new ComputeBuffer(seats_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
        seat_buffer_lod_1 = new ComputeBuffer(seats_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
        seat_buffer_lod_2 = new ComputeBuffer(seats_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
        seat_buffer_lod_3 = new ComputeBuffer(seats_count, sizeof(float) * 4 * 4, ComputeBufferType.Append);
    }

    void init_positions_buffer_seats(Matrix4x4[] seats_positions)
    {
        seat_positions_buffer = new ComputeBuffer(seats_positions.Length, sizeof(float) * 4 * 4, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        seat_positions_buffer.SetData(seats_positions);
    }

    void init_dispatch_buffer_seats(int seats_count)
    {
        dispatch_buffer_seats = new ComputeBuffer(1, sizeof(int) * 3, ComputeBufferType.IndirectArguments);
        dispatch_buffer_seats.SetData(new [] {Mathf.CeilToInt((float)seats_count/batch_size) , 1, 1 });
    }

    void init_compute_shader_seats(int seats_count)
    {
        compute_shader_seats = Instantiate(computeShader);
        
        compute_shader_seats.SetFloat("fov_dot",Mathf.Cos(renderAngle * Mathf.Deg2Rad));
        compute_shader_seats.SetFloat("lod_1_dist", lod1Distance*lod1Distance);
        compute_shader_seats.SetFloat("lod_2_dist", lod2Distance*lod2Distance);
        compute_shader_seats.SetFloat("lod_3_dist", lod3Distance*lod3Distance);
        compute_shader_seats.SetFloat("max_dist", maxDrawDistance*maxDrawDistance);
        
        compute_shader_seats.SetFloat("model_count", seats_count);
        
        compute_shader_seats.SetBuffer(kernel_index, "lod_0_buffer", seat_buffer_lod_0);
        compute_shader_seats.SetBuffer(kernel_index, "lod_1_buffer", seat_buffer_lod_1);
        compute_shader_seats.SetBuffer(kernel_index, "lod_2_buffer", seat_buffer_lod_2);
        compute_shader_seats.SetBuffer(kernel_index, "lod_3_buffer", seat_buffer_lod_3);
        compute_shader_seats.SetBuffer(kernel_index, "model_positions_buffer", seat_positions_buffer);
    }

    void init_argument_buffers_seats()
    {
        arguments_buffer_seats = new GraphicsBuffer[4];
        arguments_buffer_seats[0] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        arguments_buffer_seats[1] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        arguments_buffer_seats[2] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        arguments_buffer_seats[3] = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        
        var commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        
        commandData[0].indexCountPerInstance = chairSeatMesh0.GetIndexCount(0);
        arguments_buffer_seats[0].SetData(commandData);
        
        commandData[0].indexCountPerInstance = chairSeatMesh1.GetIndexCount(0);
        arguments_buffer_seats[1].SetData(commandData);
        
        commandData[0].indexCountPerInstance = chairSeatMesh2.GetIndexCount(0);
        arguments_buffer_seats[2].SetData(commandData);
        
        commandData[0].indexCountPerInstance = chairSeatMesh3.GetIndexCount(0);
        arguments_buffer_seats[3].SetData(commandData);
    }
    
    void init_seats_command_buffer()
    {
        command_buffer_seats = new CommandBuffer();
        command_buffer_seats.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
        
        command_buffer_seats.SetBufferCounterValue(seat_buffer_lod_0,0);
        command_buffer_seats.SetBufferCounterValue(seat_buffer_lod_1,0);
        command_buffer_seats.SetBufferCounterValue(seat_buffer_lod_2,0);
        command_buffer_seats.SetBufferCounterValue(seat_buffer_lod_3,0);
        
        command_buffer_seats.DispatchCompute(compute_shader_seats, kernel_index, dispatch_buffer_seats, 0);
        
        command_buffer_seats.CopyCounterValue(seat_buffer_lod_0,arguments_buffer_seats[0],sizeof(uint));
        command_buffer_seats.CopyCounterValue(seat_buffer_lod_1,arguments_buffer_seats[1],sizeof(uint));
        command_buffer_seats.CopyCounterValue(seat_buffer_lod_2,arguments_buffer_seats[2],sizeof(uint));
        command_buffer_seats.CopyCounterValue(seat_buffer_lod_3,arguments_buffer_seats[3],sizeof(uint));
    }
    
    void init_seats_rendering_params()
    {
        render_params_seats = new RenderParams[4];
        
        render_params_seats[0] = new RenderParams(material);
        render_params_seats[0].matProps = new MaterialPropertyBlock();
        render_params_seats[0].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params_seats[0].matProps.SetBuffer("_TRS_Array", seat_buffer_lod_0);
        render_params_seats[0].matProps.SetColor("_Col", Color.blue);
        
        render_params_seats[1] = new RenderParams(material);
        render_params_seats[1].matProps = new MaterialPropertyBlock();
        render_params_seats[1].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params_seats[1].matProps.SetBuffer("_TRS_Array", seat_buffer_lod_1);
        render_params_seats[1].matProps.SetColor("_Col", Color.yellow);
        
        render_params_seats[2] = new RenderParams(material);
        render_params_seats[2].matProps = new MaterialPropertyBlock();
        render_params_seats[2].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params_seats[2].matProps.SetBuffer("_TRS_Array", seat_buffer_lod_2);
        render_params_seats[2].matProps.SetColor("_Col", Color.red);
        
        render_params_seats[3] = new RenderParams(material);
        render_params_seats[3].matProps = new MaterialPropertyBlock();
        render_params_seats[3].worldBounds = new Bounds(Vector3.zero, Vector3.one*1000);
        render_params_seats[3].matProps.SetBuffer("_TRS_Array", seat_buffer_lod_3);
        render_params_seats[3].matProps.SetColor("_Col", Color.magenta);
    }
    
    void OnDestroy()
    {
        seat_positions_buffer?.Release();
        side_positions_buffer?.Release();
        
        seat_buffer_lod_0?.Release();
        seat_buffer_lod_1?.Release();
        seat_buffer_lod_2?.Release();
        seat_buffer_lod_3?.Release();
        side_buffer_lod_0?.Release();
        side_buffer_lod_1?.Release();
        side_buffer_lod_2?.Release();
        side_buffer_lod_3?.Release();
    }

    void Update()
    {
        Shader.SetGlobalVector("cam_pos", main_camera_transform.position - main_camera_transform.forward);
        Shader.SetGlobalVector("cam_dir", main_camera_transform.forward);
        
        
        //Graphics.ExecuteCommandBufferAsync(command_buffer_sides, ComputeQueueType.Background);
        
        render_seats();
    }

    void render_seats()
    {
        Graphics.ExecuteCommandBufferAsync(command_buffer_seats, ComputeQueueType.Background);
        Graphics.RenderMeshIndirect(render_params_seats[0], chairSeatMesh0, arguments_buffer_seats[0]);
        Graphics.RenderMeshIndirect(render_params_seats[1], chairSeatMesh0, arguments_buffer_seats[1]);
        Graphics.RenderMeshIndirect(render_params_seats[2], chairSeatMesh0, arguments_buffer_seats[2]);
        Graphics.RenderMeshIndirect(render_params_seats[3], chairSeatMesh0, arguments_buffer_seats[3]);
    }
}
