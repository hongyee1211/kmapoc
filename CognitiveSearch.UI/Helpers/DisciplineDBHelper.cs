using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public class DisciplineDBHelper
    {
        public DisciplineContext _context;


        public DisciplineDBHelper(DisciplineContext context)
        {
            this._context = context;
        }

        public ExpertsDisciplineModel[] GetAllDocuments(string discipline)
        {
            return this._context.DocumentDisciplines.Where(x => x.myExperts_Discipline == discipline).ToArray();
        }

        public ExpertsDisciplineModel[] GetAllDocuments(string[] disciplines)
        {
            return this._context.DocumentDisciplines.Where(x => disciplines.Contains(x.myExperts_Discipline)).ToArray();
        }
    }
}
