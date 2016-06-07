public class CharacterPlayerMage : CharacterPlayer
{
    protected override void Awake()
    {
        base.Awake();
        m_castAttack = new CastPlayerMageAttack(this);
        m_castSkill = new CastPlayerMageSkill(this);
    }
}