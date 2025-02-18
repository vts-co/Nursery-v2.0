using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NurseryProject.Authorization
{
    public static class MachineDataService
    {

        public static string CreateMD5(string input) //MD5 Hashing
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string getSerialID()
        {
            string fullCode = string.Empty;
            fullCode += getProcessorID() + "n" + getMotherBoardID() + "n";

            return fullCode;
        }


        public static string getProcessorID()
        {
            string code = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                           "select * from Win32_Processor");
            foreach (ManagementObject share in searcher.Get())
            {
                code = share["processorId"].ToString();
            }
            return code;
        }
        static string getMotherBoardID()
        {
            string code = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                          "select * from Win32_BaseBoard");
            foreach (ManagementObject share in searcher.Get())
            {
                code = share["SerialNumber"].ToString();
            }
            return code;
        }

        static string getHardDiskID()
        {
            string code = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                          "select * from Win32_DiskDrive");
            foreach (ManagementObject share in searcher.Get())
            {
                code = share["Signature"].ToString();
            }
            return code;
        }
    }
}