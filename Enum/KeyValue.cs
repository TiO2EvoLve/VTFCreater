namespace VTFCreater.Enum;

//压缩格式
public enum Formats
{
    RGBA8888, 
    ABGR8888, 
    RGB888, 
    BGR888, 
    RGB565, 
    I8, 
    IA88, 
    A8,
    RGB888xBLUESCREEN,
    BGR888xBLUESCREEN,
    ARGB8888, BGRA8888, 
    DXT1,
    DXT3,
    DXT5, 
    BGRX8888,
    BGR565, 
    BGRX5551, 
    BGRA4444,
    DXT1xONEBITALPHA,
    BGRA5551,
    UV88,
    UVWQ8888, 
    RGBA16161616F, 
    RGBA16161616,
    UVLX8888
}

public enum ShaderType
{
    texture,
    model
}

public enum SizeClamp
{
    x16,
    x32,
    x64,
    x128,
    x512,
    x1024,
    x2048,
    x4096
}