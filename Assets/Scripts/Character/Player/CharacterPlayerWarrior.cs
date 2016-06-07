public class CharacterPlayerWarrior : CharacterPlayer
{
    protected override void Awake()
    {
        base.Awake();
        m_castAttack = new CastPlayerWarriorAttack(this);
        m_castSkill = new CastPlayerWarriorSkill(this);
    }
}