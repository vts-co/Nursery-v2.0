using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Utilities
{
    public class GenerateRandomCode
    {
        public string GetRandomCode()
        {
            var chars = "012346789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new String(stringChars);
        }
        public string GenerateStudentCodeRandom()
        {
            string randomNumber = GetRandomCode();
            if (UniqueStudentCodeExist(randomNumber))
                GenerateStudentCodeRandom();
            return randomNumber;
        }
        public bool UniqueStudentCodeExist(string id)
        {
            var dbContext = new almohandes_DbEntities();

            var User = dbContext.Students.Where(m => m.Code == id).Select(m => m.Code).FirstOrDefault();
            if (!string.IsNullOrEmpty(User))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GenerateEmployeeCodeRandom()
        {
            string randomNumber = GetRandomCode();
            if (UniqueEmployeeCodeExist(randomNumber))
                GenerateEmployeeCodeRandom();
            return randomNumber;
        }
        public bool UniqueEmployeeCodeExist(string id)
        {
            var dbContext = new almohandes_DbEntities();

            var User = dbContext.Employees.Where(m => m.Code == id).Select(m => m.Code).FirstOrDefault();
            if (!string.IsNullOrEmpty(User))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}