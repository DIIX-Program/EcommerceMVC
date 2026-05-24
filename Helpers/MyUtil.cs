using System.Text;

namespace EcommerceMVC.Helpers
{
	public class MyUtil
	{
		public static string UploadHinh(IFormFile hinh, string folder)
		{
			try
			{
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(hinh.FileName);
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, fileName);
				using (var myfile = new FileStream(fullPath, FileMode.Create))
				{
					hinh.CopyTo(myfile);
				}
				return fileName;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}

        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhnujmikolpQAZWSXEDCRFVTGBYHNUJMIKOLP!";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
	}

    public static class HashExtensions
    {
        public static string ToMd5Hash(this string str, string? salt)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str + salt);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
