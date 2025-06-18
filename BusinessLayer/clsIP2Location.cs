using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLayer
{
    public class clsIP2Location
    {
        enum enMode { Add, Update }

        long _ID;
        enMode _Mode;
        public long Id { get { return _ID; } }
        public long IpFrom { get; set; }
        public long IpTo { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public DTOIP2Location DTO { get { return new DTOIP2Location(this._ID, this.IpFrom, this.IpTo, this.CountryCode, this.CountryName); } }
        public clsIP2Location(long IpFrom, long IpTo, string CountryCode, string CountryName)
        {
            this.IpFrom = IpFrom;
            this.IpTo = IpTo;
            this.CountryCode = CountryCode;
            this.CountryName = CountryName;
            _Mode = enMode.Add;

        }

        clsIP2Location(DTOIP2Location LocationItem)
        {
            this._ID = LocationItem.Id;
            this.IpFrom = LocationItem.IpFromBigInt;
            this.IpTo = LocationItem.IpToBigInt;
            this.CountryCode = LocationItem.CountryCode;
            this.CountryName = LocationItem.CountryName;
            _Mode = enMode.Update;

        }


        async Task<bool> _Add()
        {
            this._ID = await ConnectionLayer.clsIP2Location.AddIp2LocationAsync(this.DTO);
            return _ID != -1;
        }

        async Task<bool> _Update()
        {
            return await ConnectionLayer.clsIP2Location.UpdateIp2Location(this.DTO);
        }


        public async Task<bool> Save()
        {
            bool result;
            if (_Mode == enMode.Add)
            {
                result = await _Add();

                if (result) {

                    this._Mode = enMode.Update;
                }
            }

            else
            {
                result = await _Update();
            }

            return result;
        }


        public static async Task<clsIP2Location?> FindByIpAddress(long IpAddress)
        {

            DTOIP2Location? item = await ConnectionLayer.clsIP2Location.GetIp2Location(IpAddress);

            if (item == null) { return null; }

            return new clsIP2Location(item);



        }


        public static async Task<clsIP2Location?> FindbyUniqID(long ID)
        {

            DTOIP2Location? item = await ConnectionLayer.clsIP2Location.GetIp2LocationById(ID);

            if (item == null) { return null; }

            return new clsIP2Location(item);



        }


        public static async Task<List<DTOIP2Location>?> GetAll()
        {
            return await ConnectionLayer.clsIP2Location.GetAllIp2Location();
        }


        public static async Task<bool> Delete(int Id)
        {
            return await ConnectionLayer.clsIP2Location.DeleteIp2Location(Id);



        }


        private static BigInteger RoundToNearestInteger(BigInteger dividend, BigInteger divisor)
        {
            if (divisor <= 0)
                throw new ArgumentException("Divisor must be positive");

            if (dividend == 0)
                return 0;

            BigInteger absDividend = BigInteger.Abs(dividend);
            BigInteger quotient = absDividend / divisor;
            BigInteger remainder = absDividend % divisor;

            // Round up if remainder >= divisor/2
            if (remainder + remainder >= divisor)
            {
                quotient++;
            }

            return dividend.Sign * quotient;
        }
        public static BigInteger ParseScientificToBigInteger(string input)
        {
            // Split input into base and exponent parts
            string[] parts = input.Split('e', 'E');
            if (parts.Length != 2)
                throw new FormatException("Invalid scientific notation format");

            string basePart = parts[0].Trim();
            string exponentPart = parts[1].Trim();

            // Parse exponent
            if (!int.TryParse(exponentPart, NumberStyles.AllowLeadingSign,
                             CultureInfo.InvariantCulture, out int exponent))
                throw new FormatException("Invalid exponent format");

            // Handle base part
            int decimalIndex = basePart.IndexOf('.');
            int fractionalDigits = 0;
            string integerDigits;
            int sign = 1;

            // Extract sign if present
            if (basePart.StartsWith("-"))
            {
                sign = -1;
                basePart = basePart.Substring(1);
            }
            else if (basePart.StartsWith("+"))
            {
                basePart = basePart.Substring(1);
            }

            // Process decimal point
            if (decimalIndex >= 0)
            {
                fractionalDigits = basePart.Length - decimalIndex - 1;
                integerDigits = basePart.Remove(decimalIndex, 1);
            }
            else
            {
                integerDigits = basePart;
            }

            // Remove leading zeros (BigInteger.Parse doesn't require this but it's cleaner)
            integerDigits = integerDigits.TrimStart('0');
            if (string.IsNullOrEmpty(integerDigits))
                integerDigits = "0";

            // Parse to BigInteger
            BigInteger baseValue = BigInteger.Parse(integerDigits, CultureInfo.InvariantCulture);
            baseValue *= sign;

            // Calculate adjusted exponent
            int adjustedExponent = exponent - fractionalDigits;

            // Apply exponentiation
            if (adjustedExponent > 0)
            {
                return baseValue * BigInteger.Pow(10, adjustedExponent);
            }
            else if (adjustedExponent < 0)
            {
                // Round to nearest integer for negative exponents
                BigInteger divisor = BigInteger.Pow(10, -adjustedExponent);
                return RoundToNearestInteger(baseValue, divisor);
            }

            return baseValue; // exponent == 0
        }
        public static List<DTOIP2Location> ReadCsvFileSimple(string filePath)
        {

            int Counter = 0;

            var records = new List<DTOIP2Location>();

            // Read all lines from the CSV file
            var lines = File.ReadAllLines(filePath);

            // Skip header row if exists
            var dataLines = lines;

            foreach (string line in dataLines)
            {  
                Counter++;
                var values = line.Split(',');
             
                if (values.Length >= 4)
                {
                    records.Add(new DTOIP2Location
                    {
                  
                        IpFromBigInt = long.Parse(values[0].ToString()),
                        IpToBigInt = long.Parse(values[1].ToString()),
                        CountryCode = values[2].Trim(),
                        CountryName = values[3].Trim()
                    });
                }
                if(Counter==10000)return records;
            }

            return records;
        }
        private static void LogValidationErrors(List<string> errors, string filePath, int validCount, int totalCount)
        {
            string logFilePath = Path.ChangeExtension(filePath, ".log");

            using (var writer = new StreamWriter(logFilePath))
            {
                writer.WriteLine($"CSV Validation Report for: {Path.GetFileName(filePath)}");
                writer.WriteLine($"Generated: {DateTime.Now}");
                writer.WriteLine("==========================================");
                writer.WriteLine($"Total rows processed: {totalCount}");
                writer.WriteLine($"Valid rows: {validCount}");
                writer.WriteLine($"Invalid rows: {errors.Count}");
                writer.WriteLine("------------------------------------------");
                writer.WriteLine("Validation Errors:");

                foreach (var error in errors)
                {
                    writer.WriteLine(error);
                }
            }

            Console.WriteLine($"Validation complete. {errors.Count} errors found. See {logFilePath} for details.");
        }


    } 

}
