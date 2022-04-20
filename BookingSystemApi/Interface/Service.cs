using BookingSystemApi.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnnouncementLibrary;
using BookingSystemApi.Authentication;

namespace BookingSystemApi.Interface
{
    public class Service : IResRepo, IAnnounceRepo, ITableRepo, IBookRepo,IMailRepo
    {
        private readonly RestaurantBookingContext _context;
        public static IWebHostEnvironment _environment;
        public Service(IWebHostEnvironment environment, RestaurantBookingContext context)
        {
            _environment = environment;
            _context = context;

        }
        public async Task<int> AddRes(ResAddModel res)
        {
            try
            {
                var resExist = _context.Restaurants.Where(x => x.Name == res.Name).FirstOrDefault();
                if (resExist != null)
                {
                    return 0;
                }
                Restaurant newRes = new Restaurant();
                if (res.Image != null)
                {
                    var fileName = Path.GetFileName(res.Image.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images\\Restaurant\\", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))

                    {
                        await res.Image.CopyToAsync(fileStream);
                    }

                    newRes.Name = res.Name;
                    newRes.Address = res.Address;
                    newRes.Phone = res.Phone;
                    newRes.Image = fileName;

                    newRes.Status = 2;
                    newRes.Description = res.Description;
                    newRes.OperationStart = res.OperationStart;
                    newRes.OperationEnd = res.OperationEnd;
                    _context.Restaurants.Add(newRes);
                    await _context.SaveChangesAsync();

                }

                List<Table> table1 = new List<Table>();
                Table newtable = new Table();
                newtable.TableNo = 1;
                newtable.ResId = newRes.ResId;
                newtable.Accommodate = 0;
                newtable.Status = 1;
                table1.Add(newtable);
                await UpdateTable(table1);


                return newRes.ResId;
            } catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<PagedList<List<Restaurant>>> GetActiveRestaurants(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Restaurants.Where(x => x.Status == 1).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Restaurants.Where(x => x.Status == 1).CountAsync();

            return new PagedList<List<Restaurant>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<PagedList<List<ReturnResWithStatus>>> GetAllRestaurants(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Restaurants.Include(x => x.StatusNavigation)
                .Select(x => new ReturnResWithStatus
                {
                    ResId = x.ResId,
                    Address = x.Address,
                    Phone = x.Phone,
                    Image = x.Image,
                    Description = x.Description,
                    Name = x.Name,
                    OperationEnd = x.OperationEnd,
                    OperationStart = x.OperationStart,
                    restaurantStatus = x.StatusNavigation
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Restaurants.CountAsync();

            return new PagedList<List<ReturnResWithStatus>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<Restaurant> GetRestaurantById(int ResId)
        {
            var restaurant = await _context.Restaurants.FindAsync(ResId);
            return restaurant;
        }

        public async Task<List<RestaurantStatus>> GetRestaurantStatuses()
        {
            var restaurantStatuses = await _context.RestaurantStatuses.Where(x => x.StatusName == "active" || x.StatusName == "inactive").ToListAsync();
            return restaurantStatuses;
        }

        public async Task<List<RestaurantStatus>> GetAllRestaurantStatuses()
        {
            var restaurantStatuses = await _context.RestaurantStatuses.ToListAsync();
            return restaurantStatuses;
        }

        public async Task UpdateRestaurant(RestaurantUpdateRequest restaurantUpdateRequest)
        {
            if (_context.Restaurants.Any(x => x.ResId == restaurantUpdateRequest.ResId))
            {
                Restaurant UpdatedRestaurant = new Restaurant();
                if (restaurantUpdateRequest.ImageFile != null)
                {
                    var a = _environment.WebRootPath;
                    var fileName = Path.GetFileName(restaurantUpdateRequest.ImageFile.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images\\Restaurant\\", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))

                    {
                        await restaurantUpdateRequest.ImageFile.CopyToAsync(fileStream);
                    }
                    restaurantUpdateRequest.Image = fileName;
                }
                var getDbRes = _context.Restaurants.Where(x => x.ResId == restaurantUpdateRequest.ResId).AsNoTracking().FirstOrDefault();

                UpdatedRestaurant.ResId = restaurantUpdateRequest.ResId;
                UpdatedRestaurant.Name = restaurantUpdateRequest.Name;
                UpdatedRestaurant.Address = restaurantUpdateRequest.Address;
                UpdatedRestaurant.Phone = restaurantUpdateRequest.Phone;
                UpdatedRestaurant.Description = restaurantUpdateRequest.Description;
                UpdatedRestaurant.OperationStart = restaurantUpdateRequest.OperationStart;
                UpdatedRestaurant.OperationEnd = restaurantUpdateRequest.OperationEnd;
                UpdatedRestaurant.Image = restaurantUpdateRequest.Image;
                UpdatedRestaurant.Status = restaurantUpdateRequest.Status;

                try
                {
                    _context.Entry(UpdatedRestaurant).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }catch(Exception e)
                {

                }
            }
        }


        ////Announcement Service
        public async Task<PagedList<List<ReturnAnnouncement>>> GetAllAnnouncement(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Announcements
                .Select(x => new ReturnAnnouncement
                {
                    Title = x.Title,
                    Description = x.Description,
                    Username = x.User.UserName,
                    Date = x.Date
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Announcements.CountAsync();

            return new PagedList<List<ReturnAnnouncement>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }
        public async Task PostAnnouncement(AnnouncementAddModel announcementAddModel, string username)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
            Announcement NewAnnouncement = new Announcement();

            NewAnnouncement.Title = announcementAddModel.Title;
            NewAnnouncement.Description = announcementAddModel.Description;
            NewAnnouncement.Date = DateTime.Now;
            NewAnnouncement.UserId = getUserId.Id;
            

            _context.Announcements.Add(NewAnnouncement);
            await _context.SaveChangesAsync();
            AnnouncementClass announcementClass = new AnnouncementClass();
            announcementClass.Id = NewAnnouncement.Id;
            announcementClass.Title = NewAnnouncement.Title;
            announcementClass.Description = NewAnnouncement.Description;
            
            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Publish(announcementClass);


        }

        /////Table
        public async Task<List<Table>> GetAllTablesByRestaurantId(int resId)
        {
            
            List<Table> tableList = await _context.Tables.Where(x => x.ResId == resId).OrderBy(x => x.TableNo).ToListAsync();
            return tableList;
        }

        public async Task<List<Table>> GetTableWithDateTime(BookDateWithResId bookDateWithResId)
        {
            var getstartDate = Convert.ToDateTime(bookDateWithResId.startDate);
            var getendDate = Convert.ToDateTime(bookDateWithResId.endDate);
            List<int> bookingList = _context.Bookings
                .Where(x => x.Status != 4 && x.Status != 5 && x.ResId == bookDateWithResId.resId)
                .Where(x => (getstartDate >= x.StartDate && getstartDate <= x.EndDate) || (getendDate >= x.StartDate && getendDate < x.EndDate)
                || (x.StartDate >= getstartDate && x.StartDate <= getendDate) || (x.EndDate >= getstartDate && x.EndDate <= getendDate))
                //.Where(x => getendDate >= x.StartDate && getendDate < x.EndDate)
                .Select(x => x.TableId)
                .ToList();
            List<Table> tableList = await _context.Tables.Where(x => x.ResId == bookDateWithResId.resId && x.Status!=2).ToListAsync();

            foreach (Table t in tableList.ToList())
            {
                foreach (int id in bookingList)
                {
                    if (t.TableId == id)
                    {
                        tableList.Remove(t);
                    }
                }
            }
            return tableList;
        }



        public async Task UpdateTable(List<Table> tableList)
        {
            foreach (Table table in tableList)
            {

                if (table.TableId == 0)
                {
                    Table addTable = new Table
                    {
                        TableNo = table.TableNo,
                        Accommodate = table.Accommodate,
                        ResId = table.ResId,
                        Status = 1
                    };
                    _context.Tables.Add(addTable);
                    var getres = _context.Restaurants.Where(x => x.ResId == table.ResId).FirstOrDefault();
                    getres.Status = 1;
                    _context.Restaurants.Update(getres);

                }
                else
                {
                    var result = _context.Tables.Where(x => x.TableId == table.TableId).FirstOrDefault();
                    if (result.Status == 2)
                    {
                        result.Status = 1;
                    }
                    result.Accommodate = table.Accommodate;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Table>> GetTableByResId(int resId)
        {
            var tableList = await _context.Tables.Where(x => x.ResId == resId && x.Status!=2).OrderBy(x => x.TableNo).ToListAsync();
            if (tableList.Count == 0)
            {
                var gettable1 = _context.Tables.Where(x => x.ResId == resId && x.TableNo == 1).FirstOrDefault();
                gettable1.Status = 1;
                _context.Tables.Update(gettable1);
                await _context.SaveChangesAsync();
                List<Table> tableList1 = new List<Table>();
                tableList1.Add(gettable1);
                return tableList1;
            }
            return tableList;
        }

        public async Task<List<Table>> GetQtyTableByResId(int resId)
        {
            var tableList = await _context.Tables.Where(x => x.ResId == resId && x.Status != 2).OrderBy(x => x.TableNo).ToListAsync();
            return tableList;
        }

        public async Task<List<Table>> GetAllTableByResId(int resId)
        {
            var tableList = await _context.Tables.Where(x => x.ResId == resId && x.Status != 2).OrderBy(x => x.TableNo).ToListAsync();
            return tableList;
        }

        public async Task DeleteTable(int tableId)
        {

            var getTable = _context.Tables.Where(x => x.TableId == tableId).FirstOrDefault();
            getTable.Status = 2;
            getTable.Accommodate=0;
            _context.Tables.Update(getTable);
            await _context.SaveChangesAsync();
            
            //var getAmountTable = _context.Tables.Where(x => x.ResId == getTable.ResId && x.Status != 2).AsNoTracking().ToList();
            //var count = getAmountTable.Count;
            //var getRes = _context.Restaurants.Where(x => x.ResId == getTable.ResId).FirstOrDefault();
            //if (getRes.AmountSlot == count)
            //{
            //    getRes.Status = 1;
            //    _context.Restaurants.Update(getRes);
            //    await _context.SaveChangesAsync();
            //}

        }



        //Booking 
        public async Task<PagedList<List<ReturnPendingBooking>>> GetPendingBooking(PaginationFilter paginationFilter)
        {  
            var pagedata = await _context.Bookings.OrderBy(x => x.StartDate)
                .Where(x => x.Status == 1)
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo=x.Table.TableNo,
                    status= x.StatusNavigation.StatusName,
                    request=x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.Where(x => x.Status == 1).CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<Response> AddBooking(AddBookingModel addBookingModel,string username)
        {

            var getstartDate = Convert.ToDateTime(addBookingModel.StartDate);
            var getendDate = Convert.ToDateTime(addBookingModel.EndDate);
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
            var getUserBookingList = _context.Bookings.Where(x => x.UserId == getUserId.Id && x.StartDate.Date==getstartDate.Date).ToList();
            if (getUserBookingList.Count!=0)
            {
                Response response = new Response();
                response.Status = "AlreadyBooked";
                return response;
            }
            
            var findbooking = _context.Bookings.Where(x => x.TableId == addBookingModel.TableId && x.Status!=4 && x.Status!=5)
                .Where(x => (getstartDate >= x.StartDate && getstartDate <= x.EndDate) || (getendDate >= x.StartDate && getendDate < x.EndDate)
                || (x.StartDate >= getstartDate && x.StartDate <= getendDate) || (x.EndDate >= getstartDate && x.EndDate <= getendDate)).FirstOrDefault();

            if (findbooking == null)
            {

                Booking booking = new Booking();
                booking.ResId = addBookingModel.ResId;
                booking.StartDate = Convert.ToDateTime(addBookingModel.StartDate);
                booking.EndDate = Convert.ToDateTime(addBookingModel.EndDate);
                booking.TableId = addBookingModel.TableId;
                booking.Request = addBookingModel.Request;
                booking.UserId = getUserId.Id;
                booking.Status = 1;
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                var bus = RabbitHutch.CreateBus("host=localhost");
                var getbooking = _context.Bookings.Include(x => x.Table).Include(x => x.Res).Where(x => x.BookId == booking.BookId).FirstOrDefault();
                BookingClass bookingClass = new BookingClass();
                bookingClass.BookId = getbooking.BookId;
                bookingClass.StartDate = getbooking.StartDate;
                bookingClass.EndDate = getbooking.EndDate;
                bookingClass.Request = getbooking.Request;
                bookingClass.TableNo = getbooking.Table.TableNo;
                bookingClass.ResName = getbooking.Res.Name;
                bookingClass.Email = getbooking.User.Email;
                bus.PubSub.Publish(bookingClass, "Add");

                Response response = new Response();
                response.Status = "Success";
                return response;
            }
            else
            {
                Response response = new Response();

                response.Status = "BookedByOther";
                return response;
            }
            

        }

        public async Task<Response> SetConfirmBooking(int BookId)
        {
            var getBooking = _context.Bookings.Include(x=>x.Table).Include(x=>x.Res).Include(x=>x.User).Where(x=>x.BookId==BookId).FirstOrDefault();
            Response response = new Response();
            if (getBooking != null)
            {
                
                if (getBooking.Status != 1)
                {
                    response.Status = "Modified";
                    response.Message = $"Booking Id,{getBooking.BookId} has been cancelled by user, so it is not approved";
                }
                else
                {
                    getBooking.Status = 2;
                    _context.Bookings.Update(getBooking);
                    await _context.SaveChangesAsync();
                    response.Status = "Success";
                    response.Message = "Booking has been approved";
                    var bus = RabbitHutch.CreateBus("host=localhost");
                    BookingClass bookingClass = new BookingClass();
                    bookingClass.BookId = getBooking.BookId;
                    bookingClass.StartDate = getBooking.StartDate;
                    bookingClass.EndDate = getBooking.EndDate;
                    bookingClass.Request = getBooking.Request;
                    bookingClass.TableNo = getBooking.Table.TableNo;
                    bookingClass.ResName = getBooking.Res.Name;
                    bookingClass.Email = getBooking.User.Email;
                    bus.PubSub.Publish(bookingClass, "Confirm");
                }
                //EmailHelper emailHelper = new EmailHelper();
                //bool emailResponse = emailHelper.SendConfirmBookingEmail(getBooking.User.Email, getBooking);
            }
            else
            {
                response.Status = "Not Found";
                response.Message = "Booking Not Found";
            }
            return response;
        }

        public async Task<Response> SetConfirmDate(int BookId)
        {
            var getBooking = await _context.Bookings.FindAsync(BookId);
            Response response = new Response();
            if (getBooking != null)
            {
                if (getBooking.Status != 1)
                {
                    response.Status = "Modified";
                    response.Message = $"Booking Id,{getBooking.BookId} has been cancelled by user, so it is not validated";
                }
                else
                {
                    response.Status = "Success";
                    response.Message = "Booking has been validated";
                    getBooking.ConfirmDate = DateTime.Now;
                    getBooking.Status = 3;
                    _context.Bookings.Update(getBooking);
                    await _context.SaveChangesAsync();

                }

            }
            else
            {
                response.Status = "Not Found";
                response.Message = "Booking Not Found";
            }
            return response;
        }

        public async Task CancelBooking(int BookId)
        {
            var getBooking = await _context.Bookings.FindAsync(BookId);
            if (getBooking != null)
            {
                getBooking.Status = 4;
                _context.Bookings.Update(getBooking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> searchPendingBooking(string searchText, PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.Include(x => x.StatusNavigation).Include(x => x.User).
                Include(x => x.Res).Include(x => x.Table).OrderBy(x => x.StartDate)
                .Where(x => x.Status == 1 && x.BookId == Convert.ToInt32(searchText))
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.Where(x => x.Status == 1 && x.BookId == Convert.ToInt32(searchText)).CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<ReturnBookingWithIntStatus> GetBookingById(int bookId)
        {
            var booking = _context.Bookings.Include(x => x.StatusNavigation)
                    .Include(x => x.Res).Include(x => x.Table)
                    .Where(x=>x.BookId==bookId)
                    .Select(x => new ReturnBookingWithIntStatus
                    {
                        bookingId = x.BookId,
                        startDate = x.StartDate,
                        endDate = x.EndDate,
                        res = x.Res,
                        username = x.User.UserName,
                        table = x.Table,
                        status = x.Status,
                        request = x.Request
                    }).FirstOrDefault();
            return booking;
        }

        public async Task<List<BookingStatus>> GetBookingStatuses()
        {
            var bookingstatuses = await _context.BookingStatuses.Where(x=>x.StatusName !="expire").ToListAsync();
            return bookingstatuses;
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> GetBookingByIdWithPageList(string searchText, PaginationFilter paginationFilter)
        {
            int convertedBookingId;
            bool result = Int32.TryParse(searchText, out convertedBookingId);
            if (result)
            {
                var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                    .Where(x => x.BookId == convertedBookingId)
                    .Select(x => new ReturnPendingBooking
                    {
                        bookingId = x.BookId,
                        startDate = x.StartDate,
                        endDate = x.EndDate,
                        resName = x.Res.Name,
                        username = x.User.UserName,
                        tableNo = x.Table.TableNo,
                        status = x.StatusNavigation.StatusName,
                        request = x.Request
                    })
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize).ToListAsync();
                var totalRecords = await _context.Bookings.Where(x => x.BookId==convertedBookingId).CountAsync();

                return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
            }
            else
            {
                var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                    .Where(x => x.BookId == 0)
                    .Select(x => new ReturnPendingBooking
                    {
                        bookingId = x.BookId,
                        startDate = x.StartDate,
                        endDate = x.EndDate,
                        resName = x.Res.Name,
                        username = x.User.UserName,
                        tableNo = x.Table.TableNo,
                        status = x.StatusNavigation.StatusName,
                        request = x.Request
                    })
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize)
                    .ToListAsync();
                var totalRecords = await _context.Bookings.Where(x => x.Status == 1).CountAsync();

                return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
            }
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> GetBookingByUsernameWithPageList(string searchText, PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                .Where(x => x.User.UserName == searchText)
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.Where(x => x.User.UserName == searchText).CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> GetBookingWithPageList(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)      
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<CheckExistBooking> UpdateBookingDateTime(UpdateBookingDateTime updateBookingDateTime)
        {
           
            var getstartDate = Convert.ToDateTime(updateBookingDateTime.startDate);
            var getendDate = Convert.ToDateTime(updateBookingDateTime.endDate);
            CheckExistBooking checkExistBooking = new CheckExistBooking();
            var findbooking = _context.Bookings.Where(x => x.TableId == updateBookingDateTime.tableId && x.Status!=4 && x.Status!=5)
                .Where(x => (getstartDate >= x.StartDate && getstartDate <= x.EndDate) || (getendDate >= x.StartDate && getendDate < x.EndDate)
                || (x.StartDate >= getstartDate && x.StartDate <= getendDate) || (x.EndDate >= getstartDate && x.EndDate <= getendDate)).FirstOrDefault();
            
            if (findbooking == null)
            {
                var booking = _context.Bookings.Where(x => x.BookId == updateBookingDateTime.bookingId).FirstOrDefault();
                booking.StartDate = getstartDate;
                booking.EndDate = getendDate;
                booking.TableId = updateBookingDateTime.tableId;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
               
                checkExistBooking.checkExist = true;
                return checkExistBooking;
            }
            else
            {  
                checkExistBooking.checkExist = false;
                return checkExistBooking;
            }
        }

        public async Task UpdateBookingRequestStatus(UpdateBookingRequestStatus updateBookingRequestStatus)
        {
            var booking = _context.Bookings.Where(x => x.BookId == updateBookingRequestStatus.bookingId).FirstOrDefault();
            booking.Request = updateBookingRequestStatus.request;

            if (booking.Status == 3 && updateBookingRequestStatus.StatusId !=3)
            {
                booking.ConfirmDate = null;
            }
            else if(booking.Status!=3 && updateBookingRequestStatus.StatusId == 3)
            {
                booking.ConfirmDate = DateTime.Now;
            }
            booking.Status = updateBookingRequestStatus.StatusId;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }


        //mail
        public async Task SendAnnouncement(AnnouncementClass announcement)
        {
            var userList = _context.AspNetUsers.Where(x => x.EmailConfirmed == true).ToList();
            EmailHelper emailHelper = new EmailHelper();
            foreach (AspNetUser user in userList)
            {
                emailHelper.SendAnnouncementEmail(user.Email, announcement);
            }
        }

        public async Task SendPendingMail(BookingClass booking)
        {
           
            EmailHelper emailHelper = new EmailHelper();
            emailHelper.SendUserBookingEmail(booking);

        }

        public async Task SendConfirmMail(BookingClass booking)
        {
            
            EmailHelper emailHelper = new EmailHelper();
            emailHelper.SendConfirmBookingEmail(booking);

        }








    }
}


