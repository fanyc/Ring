public class CharacterPlayerMage : CharacterPlayer
{
    public new static float AttackPerSecond
    {
        get
        {
            return 1.867f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_castAttack = new CastPlayerMageAttack(this);
        m_castSkill = new CastPlayerMageSkill(this);
    }

    protected override string GetRunAnimation()
    {
        return "run_02";
    }
}