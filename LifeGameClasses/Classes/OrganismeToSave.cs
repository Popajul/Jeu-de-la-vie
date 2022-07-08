
namespace LifeGameClasses.Classes
{
    public class OrganismeToSave : List<CelluleToSave>
    {
       public string Name { get; set; }
       public Guid Id { get; set; }   
       public OrganismeToSave(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }
    }
}
