using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public class SubscribeDBHelper
    {
        public SubscribeContext _context;


        public SubscribeDBHelper(SubscribeContext context)
        {
            this._context = context;
        }

        public SubscribeSearchModel AddSearchQuery(string userId, string userType, string givenName, string query, int count)
        {
            SubscribeSearchModel searchQuery = new SubscribeSearchModel();
            searchQuery.userId = userId;
            searchQuery.userType = userType;
            searchQuery.givenName = givenName;
            searchQuery.query = query;
            searchQuery.documentCount = count;
            this._context.Search.Add(searchQuery);
            try
            {
                this._context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
            }
            return searchQuery;
        }

        public void DeleteSearchSubscription(string userId, string query)
        {
            SubscribeSearchModel entry = this._context.Search.FirstOrDefault(x => x.userId.Equals(userId) && x.query.Equals(query));
            if (entry != null)
            {
                this._context.Search.Remove(entry);
            }
            this._context.SaveChanges();
        }

        public List<SubscribeSearchModel> GetAllUserSubscriptions(string userId)
        {
            return this._context.Search.Where(x => x.userId.Equals(userId)).ToList();
        }

        public Boolean CheckIfSubscribed(string userId, string query)
        {
            return this._context.Search.Any(x => x.userId.Equals(userId) && x.query.Equals(query));
        }
    }
}
