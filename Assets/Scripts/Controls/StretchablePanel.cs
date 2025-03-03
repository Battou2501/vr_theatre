using UnityEngine;

[ExecuteAlways]
public class StretchablePanel : MonoBehaviour
{
    private static readonly int panel_width = Shader.PropertyToID("_PanelWidth");
    private static readonly int panel_height = Shader.PropertyToID("_PanelHeight");

    [SerializeField]
    private Renderer _renderer;

    [SerializeField][Range(0,10)]
    private float _width;
    
    [SerializeField][Range(0,10)]
    private float _height;

    private float _currentWidth;
    private float _currentHeight;
    
    private MaterialPropertyBlock _materialPropertyBlock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void UpdatePropertyBlock()
    {
        _materialPropertyBlock.SetFloat(panel_width, _currentWidth);
        _materialPropertyBlock.SetFloat(panel_height, _currentHeight);
    }
    
    private void InitializePropertyBlock()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _materialPropertyBlock.SetFloat(panel_width, _width);
        _materialPropertyBlock.SetFloat(panel_height, _height);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(_renderer == null)
            _renderer = GetComponent<Renderer>();
        
        if (_materialPropertyBlock == null)
            InitializePropertyBlock();
        
        if(Mathf.Approximately(_currentWidth, _width) && Mathf.Approximately(_currentHeight, _height)) return;
        
        _currentWidth = _width;
        _currentHeight = _height;

        UpdatePropertyBlock();
        
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
