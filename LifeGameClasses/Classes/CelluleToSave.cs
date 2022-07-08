
namespace LifeGameClasses.Classes
{
    public class CelluleToSave
    {
        public Guid Id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool isAlive { get; set; }
        public Guid OrganismeId { get; set; }
        public CelluleToSave(Cellule cellule, OrganismeToSave organisme)
        {
            this.Id = Guid.NewGuid();
            this.x = cellule.x;
            this.y = cellule.y;
            this.isAlive = cellule.isAlive;
            this.OrganismeId = organisme.Id;
        }
        public CelluleToSave(){}
    }
}
