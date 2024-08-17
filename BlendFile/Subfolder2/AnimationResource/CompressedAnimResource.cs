using System.Numerics;

public class CompressedAnimResource : AnimResourceBase
{
    uint flags;
    uint jointCount;
    uint frameCount;
    uint jumpCacheCount;
    float duration;
    float fps;
    float rotErrorMargin;
    float rotDiscontinuityThreshold;
    float traErrorMargin;
    float traDiscontinuityThreshold;
    float scaErrorMargin;
    float scaDiscontinuityThreshold;
    Vector3 translationMin;
    Vector3 translationMax;
    Vector3 scaleMin;
    Vector3 scaleMax;
    private long framesOffset;
    private long jumpCachesOffset;
    private long jointNameHashesOffset;
};