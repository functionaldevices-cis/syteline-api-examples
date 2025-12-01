using syteline_api_examples.Helpers;
using syteline_api_examples.Models;

namespace syteline_api_examples {

    internal class Program {

        static void Main() {

            /*********************************************************************************************/
            /* API GUIDE - PART 3 - LOADING RECORDS - EXAMPLE 2: LOADING SIMPLE IDO, WITH PAGINATION
            /*********************************************************************************************/

            // INITIALIZE THE RESTv2 API THROUGH ION, USING THE CREDENTIALS THAT YOU DOWNLOAD AFTER CREATING AN AUTHORIZED APP AND SERVICE ACCOUNT
            // ( FEEL FREE TO USE DIRECT CREDENTIALS HERE INSTEAD)

            SytelineAPI sytelineAPI = new(
                connection: new SytelineConnection(
                    APIType: "ION",
                    Config: "",
                    CredentialsION: new(
                        ti: "",
                        cn: "",
                        dt: "",
                        ci: "",
                        cs: "",
                        iu: "",
                        pu: "",
                        oa: "",
                        ot: "",
                        or: "",
                        ev: "",
                        v: "",
                        saak: "",
                        sask: ""
                    )
                )
            );

            // LOAD A SAMPLE REQUEST

            APILoadCollectionResponse response = sytelineAPI.LoadCollection(
                idoName: "ue_FIS_CustomLoadMethodExamples",
                properties: [
                    "PriceCode",
                    "Item",
                    "ListPrice",
                ],
                clm: "Example_03_LoadPricesForPricecode",
                clmParam: [
                    "PriceCode = 'Y25'",
                    "Item ASC"
                ],
                paginationMode: "propertyBased",
                readOnly: true
            );

            Console.WriteLine($"The request retrieved {response.Items.Count} items.");

        }

    }

}