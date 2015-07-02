using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.MapEnum;

namespace DesktopApp.Structure
{
    public class Problem
    {
        private String map;
        private String box;
        private String name;
        private int id;
        private int authorId;
        private Difficulty difficulty;
        private Boolean isPublic;


        public Boolean IsPublic
        {
            get { return isPublic; }
            set { isPublic = value; }
        }

        public Difficulty Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }
        
        public int AuthorId
        {
            get { return authorId; }
            set { authorId = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public String Box
        {
            get { return box; }
            set { box = value; }
        }

        public String Map
        {
            get { return map; }
            set { map = value; }
        }
    }
}
