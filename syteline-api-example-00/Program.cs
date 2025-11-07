using syteline_api_examples.Helpers;
using syteline_api_examples.Models;

namespace syteline_api_examples {

    internal class Program {

        static void Main() {

            /*********************************************************************************************/
            /* API GUIDE - PART 2 - AUTHENTICATION
            /*********************************************************************************************/

            // INITIALIZE THE RESTv2 API THROUGH ION, USING THE CREDENTIALS THAT YOU DOWNLOAD AFTER CREATING AN AUTHORIZED APP AND SERVICE ACCOUNT

            SytelineAPI sytelineAPI_ION = new(
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

            // INITIALIZE THE RESTv2 API DIRECTLY, USING YOUR REGULAR SYTELINE ACCOUNT CREDENTIALS

            SytelineAPI sytelineAPI_Direct = new(
                connection: new SytelineConnection(
                    APIType: "Direct",
                    Config: "",
                    CredentialsDirect: new(
                        Username: "",
                        Password: "",
                        BaseURL: "https://csi10c.erpsl.inforcloudsuite.com"
                    )
                )
            );

        }

    }

}