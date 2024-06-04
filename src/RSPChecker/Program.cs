using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RSP.CEC.WebClient.RspCecService;
using RSP.CEC.WebClient.RspClassifierService;
using RSPChecker.Properties;

namespace RSPChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Missing or too much parameters.");
                return;
            }

            if (args[0].ToLower().Contains("dumpclassifiers"))
            {
                DumpClassifiers();
                return;
            }

            Console.WriteLine("checking for: {0}", args[0]);

            try
            {
                using (var client = new CecClient())
                {
                    SetServiceCredentials(client);
                    var idnp = Int64.Parse(args[0]);
                    Console.WriteLine("Requesting RSP...");
                    var physicalPersonData = client.getPhysicalPersonData(idnp);
                    Console.WriteLine("Data received.");

                    if (physicalPersonData != null)
                    {
                        if(physicalPersonData.result.resultCode.GetValueOrDefault()!=0)
                        {
                            Console.WriteLine("errorCode {0}, message: {1}", physicalPersonData.result.resultCode, physicalPersonData.result.errorText);
                            return;
                        }

                        Console.WriteLine("Person Info:");
                        Console.WriteLine("idnp: {0}", physicalPersonData.person.idnp);
                        Console.WriteLine("firstName: {0}", physicalPersonData.person.firstName);
                        Console.WriteLine("lastName: {0}", physicalPersonData.person.lastName);
                        Console.WriteLine("secondName: {0}", physicalPersonData.person.secondName);
                        Console.WriteLine("birthDate: {0}", physicalPersonData.person.birthDate);
                        Console.WriteLine("citizenRM: {0}", physicalPersonData.person.citizenRM);
                        Console.WriteLine("dead: {0}", physicalPersonData.person.dead);
                        Console.WriteLine("sexCode: {0}", physicalPersonData.person.sexCode);

                        Console.WriteLine("Document:");
                        Console.WriteLine("docTypeCode: {0}", physicalPersonData.person.identDocument.docTypeCode);
                        Console.WriteLine("series: {0}", physicalPersonData.person.identDocument.series); 
                        Console.WriteLine("number: {0}", physicalPersonData.person.identDocument.number);
                        Console.WriteLine("issueDate: {0}", physicalPersonData.person.identDocument.issueDate);
                        Console.WriteLine("expirationDate: {0}", physicalPersonData.person.identDocument.expirationDate);
                        Console.WriteLine("issueLocation: {0}", physicalPersonData.person.identDocument.issueLocation);
                        Console.WriteLine("validity: {0}", physicalPersonData.person.identDocument.validity);

                        Console.WriteLine("Registrations:");
                        foreach (var registration in physicalPersonData.person.registration)
                        {
                            Console.WriteLine("regTypeCode: {0}", registration.regTypeCode);
                            Console.WriteLine("regDate: {0}", registration.regDate);
                            Console.WriteLine("expirationDate: {0}", registration.expirationDate);
                            Console.WriteLine("Address data");
                            Console.WriteLine("administrativCode: {0}", registration.address.administrativCode); 
                            Console.WriteLine("region: {0}", registration.address.region);
                            Console.WriteLine("locality: {0}", registration.address.locality);
                            Console.WriteLine("street: {0}", registration.address.street);
                            Console.WriteLine("streetCode: {0}", registration.address.streetCode);
                            Console.WriteLine("house: {0}", registration.address.house);
                            Console.WriteLine("block: {0}", registration.address.block);
                            Console.WriteLine("flat: {0}", registration.address.flat);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void DumpClassifiers()
        {
            const string signature = "WSCEC";
            try
            {
                using (var client = new ClassifierClient())
                {
                    SetServiceCredentials(client);
                    var namesResult = client.getClassifiersNames(signature);
                    if (namesResult.result.resultCode != null && namesResult.result.resultCode != 0)
                    {
                        Console.WriteLine("errorCode {0}, message: {1}", namesResult.result.resultCode, namesResult.result.errorText);
                        return;
                    }

                    foreach (var cd in namesResult.classifier)
                    {
                        var classifierData = client.getClassifier(signature, cd.name);
                        if (classifierData.res.resultCode != null && classifierData.res.resultCode != 0)
                        {
                            Console.WriteLine("errorCode {0}, message: {1}", classifierData.res.resultCode, classifierData.res.errorText);
                        }

                        SaveClassifer(cd, classifierData.classifier);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SaveClassifer(ClassifierDetails classifierDetails, ClassifierData classifierData)
        {
            var fileName = string.Format("rsp_{0}_rev{1}_{2}.txt", 
                classifierDetails.name, classifierDetails.changeId ?? -1,
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            using (var streamWriter = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), false))
            {
                foreach (var r in classifierData.row)
                {
                    if (r == null)
                    {
                        Console.WriteLine("r null");
                        continue;
                    }

                    if (r.items == null)
                    {
                        Console.WriteLine("r.items = null");
                        continue;
                    }

                    foreach (var x in r.items)
                    {
                        streamWriter.WriteLine("{0}\t{1}", x.fieldNames, x.values);
                    }
                }
            }
        }

        private static void SetServiceCredentials<T>(ClientBase<T> client) where T : class
        {
            client.ClientCredentials.UserName.UserName = Settings.Default.RspUser;
            client.ClientCredentials.UserName.Password = Settings.Default.RspPass;
        }
    }
}
