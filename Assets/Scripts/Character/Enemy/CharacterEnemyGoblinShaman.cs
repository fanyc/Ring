using UnityEngine;
using System.Collections;
using IProjectileExtension;

public class CharacterEnemyGoblinShaman : CharacterEnemy, IProjectile
{
    public string ProjectileName => "@Proj_Fireball_GoblinShaman";

    public string MuzzleName => "fx_01";
    public override TYPE Type
    {
        get { return TYPE.Normal; }
    }
    
    public override float HPFactor 
    {
        get { return 5.0f; }
    }

    public override void Init()
    {
        m_castAttack = new CastAttack(this);
        base.Init();
    }
    
    protected override void IdleThought()
    {
        if(m_castAttack?.GetTargets()?.Length > 0)
        {
            Attack();
        }
        else
        {
            State = STATE.MOVE;
        }
    }

    protected IEnumerator MOVE()
    {
        PlayAnimation(GetRunAnimation(), false, true);
        while(State == STATE.MOVE)
        {
            
            if(m_castAttack.GetTargets().Length > 0)
            {
                Attack();
                yield return null;
                break;
            }
            
            Vector3 pos = position + new Vector3(11.25f * Time.smoothDeltaTime * 0.25f * -GameManager.Instance.Direction, 0.0f);
            
            position = pos;
            
            yield return null;
        }
        
        NextState();
    }

    public override void Dead()
    {
        float reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue / (6.0f * 8.0f);
        for(int i = 0; i < 8; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", position + new Vector3(0.0f, 1.0f)).Init(reward);
        }

        base.Dead();
    }

    protected class CastAttack : Castable
    {
        private Character m_cachedTarget;

        private int m_cachedMask = 0;
        public override int TargetMask
        {
            get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Ally"); return m_cachedMask; }
        }

        public override Vector2 Rect
        {
            get
            {
                return new Vector2(8.0f * m_caster.Direction, 1.5f);
            }
        }

        public CastAttack(CharacterEnemy caster) : base(caster)
        {
        }

        public override bool Condition()
        {
            if(IsCoolTime()) return false;
            return true;
        }
        
        protected override void Prepare()
        {
            m_caster.AddAnimationEvent(Hit);
        }
        protected override IEnumerator Cast()
        {
            State = Character.STATE.CAST;
            m_caster.PlayAnimation(m_caster.GetAttackAnimation(), false, false, GameManager.Instance.PlayerSpeed);
            m_cachedTarget = GetNearestTarget(GetTargets());
            if(m_cachedTarget != null)
                while(m_caster.IsEndAnimation() != true) yield return null;
            State = Character.STATE.IDLE;
        }
        
        protected override void Release()
        {
            m_caster.RemoveAnimationEvent(Hit);
        }

        void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
        {
            if(m_cachedTarget == null) return;
            if(m_cachedTarget.State == Character.STATE.DEAD || m_cachedTarget.State == Character.STATE.NULL)
            {
                m_cachedTarget = null;
                return;
            }
            IProjectile ProjectileData = (m_caster as IProjectile);
            Projectile proj = ObjectPool<Projectile>.Spawn(ProjectileData.ProjectileName);
            float dist = (m_cachedTarget.position.x - m_caster.position.x) + Random.Range(-0.25f, 0.25f);
            MuzzleData muzzle = ProjectileData.GetMuzzleData();
            //proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, muzzle.angle);
            Vector2 pos = (Vector2)m_caster.position + muzzle.worldPosition;
            Vector2 dest = new Vector2(pos.x + 2.0f * m_caster.Direction, 0.0f);
            proj.Init((Vector3)pos, dest, ()=>
            {
                ObjectPool<Effect>.Spawn("@Effect_Fireball_OrcMage_Explosion").Init(proj.cachedTransform.position);

                Character[] targets = GetTargets((Vector3)(dest + new Vector2(Rect.x * -0.5f, 0.0f)), new Vector2(Rect.x * 0.5f, Rect.y));

                for(int i = 0; i < targets.Length; ++i)
                {
                    Character target = targets[i];
                    if(target == null) continue;

                    float distFactor = ((target.position.x - dest.x) / (Rect.x * 0.5f));
                    distFactor = (1.0f - Mathf.Abs(distFactor)) * Mathf.Sign(distFactor);
                    float knockback = 20.0f * distFactor;
                    float airborne = 0.0f;//2.3f + 4.6f * Mathf.Abs(distFactor);
                    //target.KnockBack(new Vector2(knockback, airborne));
                    target.Stun(0.2f);
                }
            });
        }
    }
}

