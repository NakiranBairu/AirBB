using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AirBB.Models.DataLayer
{
    public class QueryOptions<T> where T : class
    {
        public Expression<Func<T, bool>>? Filter { get; set; }
        public List<Expression<Func<T, object>>>? Includes { get; set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
