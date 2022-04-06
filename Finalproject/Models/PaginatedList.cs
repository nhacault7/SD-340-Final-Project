using Microsoft.EntityFrameworkCore;

namespace Finalproject.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;                                        // current page index
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);     // total page count; Ceiling example: Math.Ceiling(11/10)=2; Math.Ceiling(-11/10)=-1;

            this.AddRange(items);         // add the items to PaginatedList
        }
        //check if the previousPage or NextPage button is avalabile
        public bool HasPreviousPage => PageIndex > 1;  //if pageIndex >1, HasPreviousPage = true, else false;

        public bool HasNextPage => PageIndex < TotalPages; //if HasNextPage < TotalPages, HasNextPage = true, else false;

        //Static method:get the List of items of current page
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();//total items count
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); //return a List containing only the requested page; eg: page 1 -> take(1-10) items
            return new PaginatedList<T>(items, count, pageIndex, pageSize);//call the constructor method and return the List of items
        }
    }

}
