public class CharacterPlayerArcher : CharacterPlayer
{
    protected override void Awake()
    {
        base.Awake();
        m_castAttack = new CastPlayerArcherAttack(this);
        m_castSkill = new CastPlayerArcherSkill(this);
    }
}