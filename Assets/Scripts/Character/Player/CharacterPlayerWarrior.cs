public class CharacterPlayerWarrior : CharacterPlayer
{
    public new static float AttackPerSecond
    {
        get
        {
            return 0.467f;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        m_castAttack = new CastPlayerWarriorAttack(this);
        m_castSkill = new CastPlayerWarriorSkill(this);
    }
}