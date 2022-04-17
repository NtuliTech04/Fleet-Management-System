using FleetTours___Application.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace FleetTours___Application.BusinessLogic
{
    public class UserAccount
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<UserProfile> all()
        {
            return db.UserProfiles.ToList();
        }
        public bool add(UserProfile model)
        {
            try
            {
                db.UserProfiles.Add(model);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public bool edit(UserProfile model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public UserProfile find_by_id(int? id)
        {
            return db.UserProfiles.Find(id);
        }

        public string getGender(string id_num)
        {
            if (Convert.ToInt16(id_num.Substring(7, 1)) >= 5)
                return "Male";
            else
                return "Female";
        }
    }
}