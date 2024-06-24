using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CEC.SRV.Domain.Importer
{
    public class AlegatorData : PersonRaw
    {
        public AlegatorData()
        {
            Source = SourceEnum.Alegator;
        }

        public virtual long? AlegatorId { get; set; }

        public virtual long? AlegatorPollingStationId { get; set; }

        public virtual string DocSeria { get; set; }

        public virtual string DocNumber { get; set; }

        public virtual string StreetName { get; set; }

        public virtual string HouseNr { get; set; }

        public virtual string Apartment { get; set; }

        public virtual int PersonStatus { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (!(PersonStatus == 0 || PersonStatus == 100 || PersonStatus == 101))
                result.Add(new ValidationResult("RecordStatus nu este in 0, 100, 101", new[] { "PersonStatus" }));

            if (string.IsNullOrWhiteSpace(GetDocumentSeria()) || string.IsNullOrWhiteSpace(GetDocumentNumber()))
                result.Add(new ValidationResult("document seria sau numar nu poate fi identificat", new[] { "DocSeria", "DocNumber" }));

            if (!AlegatorPollingStationId.HasValue)
                result.Add(new ValidationResult("polling station does not have and Id"));

            if (!DateOfBirth.HasValue)
                result.Add(new ValidationResult("Data nasterii nu e specificata", new[] { "DateOfBirth" }));

            return result;
        }


        public virtual string GetPersonStatus()
        {
            string result;

            switch (PersonStatus)
            {
                case 0:
                    result = "Decedat";
                    break;
                case 100:
                case 101:
                    result = "Alegător";
                    break;
                default:
                    result = "Unknown";
                    break;
            }

            return result;
        }

        public virtual string GetApartmentSuffix()
        {
            if (string.IsNullOrWhiteSpace(Apartment))
                return null;
            var apartmentNumber = GetApartmentNumber();

            if (!apartmentNumber.HasValue)
                return null;

            string apartmetnSuffix =
                Apartment.Replace(apartmentNumber.Value.ToString(), "")
                    .Trim('\\', ' ', '/', '-', ',', '.');

            return string.IsNullOrWhiteSpace(apartmetnSuffix) ? null : apartmetnSuffix;
        }

        public virtual int? GetApartmentNumber()
        {
            if (string.IsNullOrWhiteSpace(Apartment))
                return null;

            return GetFirstNumberFrom(Apartment);
        }

        public virtual int? GetHouseNumber()
        {
            if (string.IsNullOrWhiteSpace(HouseNr) || HouseNr == "<" || GetStreetName() == ">")
                return null;

            return GetFirstNumberFrom(HouseNr);
        }

        public virtual string GetHouseSuffix()
        {
            if (string.IsNullOrWhiteSpace(HouseNr) || HouseNr == "<")
                return null;

            var homeNumber = GetHouseNumber();
            if (!homeNumber.HasValue)
                return null;

            var suffix = HouseNr.Replace(homeNumber.Value.ToString(), "").Trim('\\', ' ', '/', '-', ',', '.');

            return string.IsNullOrWhiteSpace(suffix) ? null : suffix;


        }

        public virtual string GetDocumentSeria()
        {
            if (!string.IsNullOrWhiteSpace(DocSeria) && (DocSeria.Trim() == "A" || DocSeria.Trim() == "B" || DocSeria.Trim() == "CJA"))
            {
                return DocSeria;
            }

            if (!string.IsNullOrWhiteSpace(DocNumber))
            {
                if (DocNumber.StartsWith("A") && DocNumber.Length == 9)
                    return "A";
                if (DocNumber.StartsWith("B") && DocNumber.Length == 9)
                    return "B";
                if (DocNumber.StartsWith("CJA") && DocNumber.Length == 9)
                    return "CJA";

                if (DocNumber.Contains("БМ"))
                {
                    var index = DocNumber.IndexOf("БМ");
                    return DocNumber.Substring(0, index + 2);
                }

            }

            return string.Empty;

        }

        public virtual string GetDocumentNumber()
        {
            var seria = GetDocumentSeria();

            if (string.IsNullOrWhiteSpace(seria))
                return string.Empty;

            var number = DocNumber.Replace(seria, string.Empty);

            if (seria == "A" || seria == "B")
                return number.Length == 8 ? number : string.Empty;

            if (seria == "CJA")
                return number.Length == 6 ? number : string.Empty;

            if (seria.Contains("БМ"))
                return number.Length == 6 ? number : string.Empty;

            return string.Empty;
        }

        public virtual string GetDocumentType()
        {
            var seria = GetDocumentSeria();

            if (seria == "A" || seria == "B")
                return "Buletin";

            if (seria == "CJA")
                return "F-9";

            if (seria.Contains("БМ"))
                return "Sovietic";

            return "Unknown";
        }

        public virtual string GetGender()
        {
            string result;
            switch (Gender)
            {
                case "Masculin":
                    result = "M";
                    break;
                case "Feminin":
                    result = "F";
                    break;
                default:
                    result = ">";
                    break;
            }

            return result;
        }

        private static int? GetFirstNumberFrom(string str)
        {
            var digits = new string(str.TakeWhile(Char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(digits))
                return null;

            int result;
            if (Int32.TryParse(digits, out result))
            {
                return result;
            }

            return null;
        }

        public virtual string GetStreetName()
        {

            if (string.IsNullOrWhiteSpace(StreetName) || StreetName == ">" || StreetName == "<")
            {
                return ">";
            }

            return StreetName.Replace(GetStreetType(), string.Empty);
        }


        public virtual string GetStreetType()
        {
            if (string.IsNullOrWhiteSpace(StreetName) || StreetName == ">" || StreetName == "<")
            {
                return ">";
            }

            return StreetTypeDictionary.TryGetStreetTypeFrom(StreetName);
        }
    }


    public static class StreetTypeDictionary
    {

        private static IDictionary<string, string[]> _streetDictionary = new Dictionary<string, string[]>()
        {
            //{">", new[] {">"}},
            {"bd.", new[] {"bd.", "bulevard", "bulevardul"}},
            {"șos.", new[] {"șos.", "șos", "şos.", "şos"}},
            {"str.", new[] {"str", "str.", "strada"}},
            {"str-la", new[] {"str-la", "stla", "str-lă"}},
            {"str-la înf.", new[] {"FUND", "înf"}},
            
        };


        public static string TryGetStreetTypeFrom(string streetName)
        {
            var words = streetName.Split(' ', '-', '+', '.', ',', '/', '\\', '(', ')', '[', ']');

            foreach (var word in words)
            {
                foreach (var stringse in _streetDictionary.Where(stringse => stringse.Value.Any(type => type == word)))
                {
                    return stringse.Key;
                }
            }

            return "str.";
        }
    }

}