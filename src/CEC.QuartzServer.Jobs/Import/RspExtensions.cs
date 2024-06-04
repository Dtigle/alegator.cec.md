using System;
using System.Linq;
using CEC.SRV.Domain.Importer;
using RSP.CEC.WebClient.RspCecService;

namespace CEC.QuartzServer.Jobs.Import
{
    public static class RspExtensions
    {
        public static RspRegistrationData ToRegistrationData(this RegistrationData registrationData)
        {
            var rspRegistrationData = new RspRegistrationData
            {
                DateOfRegistration = registrationData.regDate ?? DateTime.MinValue,
                RegTypeCode = registrationData.regTypeCode,
                DateOfExpiration = registrationData.expirationDate,
                Administrativecode = registrationData.address.administrativCode.Value,
                Locality = registrationData.address.locality,
                Region = registrationData.address.region,
                StreetName = registrationData.address.street,
                StreetCode = registrationData.address.streetCode.Value,
                HouseSuffix = registrationData.address.block,
                
            };

            var houseNrData = SplitAddress(registrationData.address.house, registrationData.address.block);
            rspRegistrationData.HouseNumber = houseNrData.Item1;

            rspRegistrationData.HouseSuffix = houseNrData.Item2;

            var apprtmentData = SplitAddress(registrationData.address.flat, null);

            rspRegistrationData.ApartmentNumber = apprtmentData.Item1;
            rspRegistrationData.ApartmentSuffix = apprtmentData.Item2;

            return rspRegistrationData;
        }

        private static Tuple<int?, string> SplitAddress(string houseNr, string bloc)
        {
            var resultHouseNr = GetFirstNumberFrom(houseNr);

            bloc = string.IsNullOrWhiteSpace(bloc) ? "" : bloc.Replace("<", "");

            if (resultHouseNr.HasValue)
            {
                var nonNumberHouseNr = houseNr.Replace(resultHouseNr.Value.ToString(), "");

                if (!string.IsNullOrWhiteSpace(nonNumberHouseNr))
                {
                    bloc = nonNumberHouseNr.Replace("/", "") + bloc;
                }
            }

            if (string.IsNullOrWhiteSpace(bloc))
            {
                bloc = null;
            }

            return new Tuple<int?, string>(resultHouseNr, bloc);
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
    }
}