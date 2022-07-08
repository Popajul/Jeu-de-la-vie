using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeGameClasses.Classes
{
    /// <summary>
    /// L'organisme est représenté par une liste de ligne contenant des cellules
    /// </summary>
    public class Organisme : List<List<Cellule>>
    {
        #region properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Margin { get;set; }
        #endregion
        /// <summary>
        /// ctr
        /// </summary>
        public Organisme(int X, int Y, int margin, bool setMustBeDisplayAndXy = false)
        {
            this.X = X;
            this.Y = Y;
            this.Margin = margin;
            var dimX = X + 2 * Margin;
            var dimY = Y + 2 * Margin;
            Parallel.For(0, dimY, (a) => {
                this.Add(new List<Cellule>());
                Parallel.For(0, dimX, (b)=>
                {
                    this.Last().Add(new Cellule() { isAlive = false });
                });
            });
            if (setMustBeDisplayAndXy)
                SetMustBeDisplayAndXy();
        }
       
        #region public methods

        /// <summary>
        /// Génération d'une configuration cellulaire aléatoire de l'organisme
        /// </summary>
        public void InitializeRandomly(int birthRate)
        {
            Random random = new Random();
            Parallel.ForEach(this.SelectMany(l => l.Where(c => c.MustBeDisplay)).ToList(),c=>
                c.isAlive = (random.Next(10) >= birthRate / 10) ? false : true);
        }
        
        /// <summary>
        /// Passe l'organisme dans son itération suivante
        /// </summary>
        public void GenerateNext()
        {
            Console.WriteLine("generate next iteration");
            var liste = GetCellulesCouldBeNextAlive();
            Console.WriteLine($"Calculate iteration for {liste.Count} cells");
            SetCelluleIsInlifeNext(liste);

            Parallel.ForEach(liste, c =>
            {
                c.isAlive = c.isInLifeNext ?? false;
                c.isInLifeNext = null;
            });

        }
        public async Task GenerateNextAsync()
        {
            Console.WriteLine("generate next iteration");
            var liste = GetCellulesCouldBeNextAlive();
            Console.WriteLine($"Calculate iteration for {liste.Count} cells");
            await Task.Run(()=>SetCelluleIsInlifeNext(liste));

            Parallel.ForEach(liste, c =>
            {
                c.isAlive = c.isInLifeNext ?? false;
                c.isInLifeNext = null;
            });
        }

        public void KillAll()
        {
            Parallel.ForEach(this.SelectMany(l => l.Where(c => true)).ToList(), c => c.isAlive = false);
        }
        public Organisme Duplicate()
        {
            Organisme organisme = new Organisme(this.X, this.Y,this.Margin);
            Parallel.ForEach(organisme.SelectMany(l=>l.Where(c=>true), (l,c) => new {ligne = l, cellule = c }).ToList(), a =>
            {
                int i = organisme.IndexOf(a.ligne);
                int j = a.ligne.IndexOf(a.cellule);
                a.cellule.isAlive = this[i][j].isAlive;
                a.cellule.isInLifeNext = this[i][j].isInLifeNext;
                a.cellule.MustBeDisplay = this[i][j].MustBeDisplay;
                a.cellule.x = this[i][j].x;
                a.cellule.y = this[i][j].y;
            });
            return organisme;
        }
        #endregion

        #region private methods

        private bool IsDisplayable(Cellule cellule) => (cellule.x > this.Margin) && (cellule.x <= this.X + Margin) && (cellule.y > this.Margin) && (cellule.y <= this.Y + Margin);
        /// <summary>
        ///  met à jour ou set les coordonnées des cellules
        /// </summary>
        private void SetMustBeDisplayAndXy()
        {
            Parallel.ForEach(this.SelectMany(l => l.Where(c => true), (l, c) => new { ligne = l, cellule = c }).ToList(), a =>
            {
                a.cellule.x = a.ligne.IndexOf(a.cellule) + 1;
                a.cellule.y = this.IndexOf(a.ligne) + 1;
                a.cellule.MustBeDisplay = IsDisplayable(a.cellule);
            }
            );
        }
 
        public List<Cellule> GetCellulesCouldBeNextAlive()
        {
            var tab = new (int,int)[] { (-1,-1),(-1,0),(-1,1),(0,-1),(0,0),(0,1),(1,-1),(1,0),(1,1)};
            var liste = new List<Cellule>();
            Parallel.ForEach(this.SelectMany(l => l.Where(c => c.isAlive)).ToList(), c =>
            {
                    Parallel.ForEach(tab, t =>
                    {
                        var currentCell = this[c.y - 1 + t.Item1][c.x - 1 + t.Item2];
                        try { liste.Add(currentCell); }
                        catch (ArgumentOutOfRangeException) { /**/ }
                    });
                
            }
            );
            return liste.Distinct().ToList();
        } 
        /// <summary>
        /// set islnLifeNext for each cell
        /// </summary>
        private void SetCelluleIsInlifeNext(List<Cellule> liste)
        {
             Parallel.Invoke( 
                () => Parallel.ForEach(GetDeadCells(liste), c => c.isInLifeNext = GetNombreVoisine(c) == 3),
                () => Parallel.ForEach(GetAliveCells(liste), (c) =>
                    {
                        var nombreVoisine = GetNombreVoisine(c);
                        c.isInLifeNext = (nombreVoisine == 2 || nombreVoisine == 3);
                    })
            );
        }

       
        private int GetNombreVoisine(Cellule elt)
        {
            if (elt.x == 1 || elt.x == this.X + 2 * Margin || elt.y == 1 || elt.y == this.Y + 2 * Margin)
                return 0;

            (int,int)[] tab = new (int,int)[] {(-2,-2),(-2,-1),(-2,0),(-1,-2),(-1,0),(0,-2),(0,-1),(0,0)};
            int compteurDeVoisine = 0;
            Parallel.ForEach(tab, t =>
            {
                compteurDeVoisine += this[elt.y + t.Item1][elt.x + t.Item2].isAlive ? 1 : 0;
            });
            return compteurDeVoisine;
        }
        private static List<Cellule> GetDeadCells(List<Cellule> liste)
        {          
            return liste.Where(c => !c.isAlive).ToList();
        }

        private static List<Cellule> GetAliveCells(List<Cellule> liste)
        {
            return liste.Where(c => c.isAlive).ToList();
        }
        #endregion
    }
}
