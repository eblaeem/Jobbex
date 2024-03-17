namespace Entities
{
    public class SoftwareSkill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SoftwareSkillGroupId { get; set; }

        public virtual SoftwareSkillGroup SoftwareSkillGroup { get; set; }
    }
}
