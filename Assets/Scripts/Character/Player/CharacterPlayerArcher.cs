public class CharacterPlayerArcher : CharacterPlayer
{
    public new static float AttackPerSecond
    {
        get
        {
            return 0.867f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_castAttack = new CastPlayerArcherAttack(this);
        m_castSkill = new CastPlayerArcherSkill(this);
    }

    protected override string GetRunAnimation()
    {
        return "run_02";
    }
}