using UnityEngine;
using System.Collections;

public class CharacterEnemyBossSample : CharacterEnemy
{
    public override TYPE Type
    {
        get { return TYPE.StageBoss; }
    }
    public override float HPFactor
    {
        get { return 20.0f; }
    }

    public float Range = 5.0f;

    protected Castable[] m_castSkill = new Castable[3];
    
    public override void Init()
    {
        m_castAttack = new CastEnemyMeleeAttack(this, new Vector2(Range, 1.5f), 2.0f);
        m_castSkill[0] = new CastEnemyBackStep(this);
        m_castSkill[1] = new CastSkillRampage(this);
        m_castSkill[2] = new CastSkillBite(this);

        base.Init();

        m_fWeight = 15.0f;
        
        for(int i = 0; i < m_castSkill.Length; ++i)
        {
            m_castSkill[i].SetCoolTime(5.0f);
        }
    }
    

    protected override void IdleThought()
    {
        for(int i = 1; i < m_castSkill.Length; ++i)
        {
            if(m_castSkill[i].Condition())
            {
                Cast(m_castSkill[i]);
                return;
            }
        }
        if(m_castAttack.Condition())
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
            Vector3 pos = position + new Vector3(11.25f * Time.smoothDeltaTime * 0.25f * -GameManager.Instance.Direction, 0.0f);
            
            position = pos;

            IdleThought();
            
            yield return null;
        }
        
        NextState();
    }

    public override void Beaten(float damage, DAMAGE_TYPE type, bool isSmash = false)
    {
        base.Beaten(damage, type, isSmash);
        Cast(m_castSkill[0]);
    }

    public override void Dead()
    {
        base.Dead();
    }
    

    public override void PlayBeatenAnimation()
    {
        
    }

    public override string GetDeadAnimation()
    {
        return "dead_01";
    }

    protected class CastSkillRampage : Castable
    {
        private int m_cachedMask = 0;
        public override int TargetMask
        {
            get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Ally"); return m_cachedMask; }
        }

        public override Vector2 Rect
        {
            get
            {
                return new Vector2(5.0f * m_caster.Direction, 1.5f);
            }
        }

        public override float Distance
        {
            get
            {
                return 2.0f;
            }
        }

        public CastSkillRampage(Character caster) : base(caster)
        {
        }
        
        public override bool Condition()
        {
            if(IsCoolTime()) return false;

            Character target = Castable.GetNearestTarget(m_caster, GetTargets());
            if(target == null || Mathf.Abs(target.position.x - position.x) > Distance) return false;

            return true;
        }
        
        protected override void Prepare()
        {
            m_caster.AddAnimationEvent(Hit);
        }
        protected override IEnumerator Cast()
        {
            State = Character.STATE.CAST;
            m_caster.PlayAnimation("skill_01", false, false, GameManager.Instance.PlayerSpeed);
            while(m_caster.IsEndAnimation() != true) yield return null;
            SetCoolTime(10.0f);
            State = Character.STATE.IDLE;
        }
        
        protected override void Release()
        {
            m_caster.RemoveAnimationEvent(Hit);
        }

        void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
        {
            m_caster.StartCoroutine(_hit());
        }

        IEnumerator _hit()
        {
            for(int j = 0; j < 3; ++j)
            {
                Character[] targets = GetTargets();
                int count = targets.Length;

                for(int i = 0; i < count; ++i)
                {
                    Character target = targets[i];
                    if(target == null) continue;
                    target.Beaten(2.0f, Character.DAMAGE_TYPE.ETC);
                    target.KnockBack(new Vector2(7.5f * m_caster.Direction, 0.0f));
                    target.Stun(0.2f);
                    
                    ObjectPool<Effect>.Spawn("@Effect_Hit_Enemy").Init(target.position + new Vector3(0.2f * -m_caster.Direction, 0.5f));
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    protected class CastSkillBite : Castable
    {
        private int m_cachedMask = 0;
        public override int TargetMask
        {
            get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Ally"); return m_cachedMask; }
        }

        public override Vector2 Rect
        {
            get
            {
                return new Vector2(5.0f * m_caster.Direction, 1.5f);
            }
        }

        public override float Distance
        {
            get
            {
                return 2.0f;
            }
        }

        public CastSkillBite(Character caster) : base(caster)
        {
        }
        
        public override bool Condition()
        {
            if(IsCoolTime()) return false;

            Character target = Castable.GetNearestTarget(m_caster, GetTargets());
            if(target == null || Mathf.Abs(target.position.x - position.x) > Distance) return false;

            return true;
        }
        
        protected override void Prepare()
        {
            m_caster.AddAnimationEvent(Hit);
        }
        protected override IEnumerator Cast()
        {
            State = Character.STATE.CAST;
            m_caster.PlayAnimation("skill_02", false, false, GameManager.Instance.PlayerSpeed);
            while(m_caster.IsEndAnimation() != true) yield return null;
            SetCoolTime(10.0f);
            State = Character.STATE.IDLE;
        }
        
        protected override void Release()
        {
            m_caster.RemoveAnimationEvent(Hit);
        }

        void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
        {
            Character[] targets = GetTargets();
            int count = targets.Length;

            for(int i = 0; i < count; ++i)
            {
                Character target = targets[i];
                if(target == null) continue;
                target.Beaten(2.0f, Character.DAMAGE_TYPE.ETC);
                target.KnockBack(new Vector2(7.5f * m_caster.Direction, 0.0f));
                target.Stun(0.2f);
                
                ObjectPool<Effect>.Spawn("@Effect_Hit_Enemy").Init(target.position + new Vector3(0.2f * -m_caster.Direction, 0.5f));
            }
        }
    }
}