using UnityEngine;

public interface IProjectile
{
    string ProjectileName
    {
        get;
    }

    string MuzzleName
    {
        get;
    }
}

namespace IProjectileExtension
{
    public struct MuzzleData
    {
        public float angle;
        public Vector2 worldPosition;

        public MuzzleData(float _angle, Vector2 _worldPosition)
        {
            angle = _angle;
            worldPosition = _worldPosition;
        }
    }

    public static class IProjectileMethod
    {
        public static MuzzleData GetMuzzleData(this IProjectile _base)
        {
            Spine.Bone bone = ((Character)_base).GetAnimationBone(_base.MuzzleName);
            return new MuzzleData(bone.AppliedRotation, new Vector2(bone.WorldX, bone.WorldY));
        }
    }
}