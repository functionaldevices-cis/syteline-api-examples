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

            // SAMPLE INSERT REQUESTS

            APIUpdateCollectionResponse response;

            // INSERT TWO PLANNERCODE RECORDS

            response = sytelineAPI.UpdateCollection(
                idoName: "SLPlanners",
                refreshAfterSave: true,
                changes: [
                    new(
                        action: APIUpdateCollectionRequestChangeAction.Insert,
                        properties: [
                            new(
                                name: "PlanCode",
                                value: "Z01",
                                modified: true
                            ),
                            new(
                                name: "Description",
                                value: "Test",
                                modified: true
                            ),
                            new(
                                name: "ShowInDropDownList",
                                value: "1",
                                modified: true
                            )
                        ]
                    ),
                    new(
                        action: APIUpdateCollectionRequestChangeAction.Insert,
                        properties: [
                            new(
                                name: "PlanCode",
                                value: "Z02",
                                modified: true
                            ),
                            new(
                                name: "Description",
                                value: "Test 2",
                                modified: true
                            ),
                            new(
                                name: "ShowInDropDownList",
                                value: "1",
                                modified: true
                            )
                        ]
                    )
                ]
            );

            // UPDATE ONE OF THEM

            response = sytelineAPI.UpdateCollection(
                idoName: "SLPlanners",
                refreshAfterSave: true,
                changes: [
                    new(
                        action: APIUpdateCollectionRequestChangeAction.Update,
                        properties: [
                            new(
                                name: "PlanCode",
                                value: "Z02",
                                modified: false
                            ),
                            new(
                                name: "Description",
                                value: "Test 2 Modified",
                                modified: true
                            ),
                            new(
                                name: "ShowInDropDownList",
                                value: "0",
                                modified: true
                            )
                        ]
                    )
                ]
            );

            // INSERT A USER DEFINED TYPE AND CHILD VALUES

            response = sytelineAPI.UpdateCollection(
                idoName: "UserDefinedTypes",
                changes: [
                    new(
                        action: APIUpdateCollectionRequestChangeAction.Insert,
                        properties: [
                            new(
                                name: "Name",
                                value: "MyUserDefinedColor",
                                modified: true
                            ),
                            new(
                                name: "Description",
                                value: "This is a user defined type.",
                                modified: true
                            ),
                            new(
                                nestedCollection: new(
                                    idoName: "UserDefinedTypeValues",
                                    links: [
                                        new(
                                            parentProperty: "Name",
                                            childProperty: "TypeName"
                                        )
                                    ],
                                    changes: [
                                        new(
                                            action: APIUpdateCollectionRequestChangeAction.Insert,
                                            properties: [
                                                new(
                                                    name: "TypeName",
                                                    value: "MyUserDefinedColor",
                                                    modified: true
                                                ),
                                                new(
                                                    name: "Value",
                                                    value: "Red",
                                                    modified: true
                                                ),
                                                new(
                                                    name: "Description",
                                                    value: "This is red.",
                                                    modified: true
                                                )
                                            ]
                                        ),
                                        new(
                                            action: APIUpdateCollectionRequestChangeAction.Insert,
                                            properties: [
                                                new(
                                                    name: "TypeName",
                                                    value: "MyUserDefinedColor",
                                                    modified: true
                                                ),
                                                new(
                                                    name: "Value",
                                                    value: "Green",
                                                    modified: true
                                                ),
                                                new(
                                                    name: "Description",
                                                    value: "This is green.",
                                                    modified: true
                                                )
                                            ]
                                        )
                                    ]
                                )
                            )
                        ]
                    )
                ]
            );

            // INSERT TWO PLANNER CODES USING CLEANER SYNTAX

            response = sytelineAPI.InsertRecords(
                idoName: "SLPlanners",
                records: [
                    new(
                        properties: new()
                        {
                           ["PlanCode"] = "Z03",
                           ["Description"] = "Test 3",
                           ["ShowInDropDownList"] = "1"
                        }
                    ),
                    new(
                        properties: new()
                        {
                           ["PlanCode"] = "Z04",
                           ["Description"] = "Test 4",
                           ["ShowInDropDownList"] = "1"
                        }
                    )
                ]
            );

            // UPDATE ONE OF THEM USING CLEANER SYNTAX

            response = sytelineAPI.UpdateRecords(
                idoName: "SLPlanners",
                matchingProperties: [
                    "PlanCode"
                ],
                records: [
                    new(
                        properties: new()
                        {
                           ["PlanCode"] = "Z04",
                           ["Description"] = "Test 4 Updated",
                           ["ShowInDropDownList"] = "0"
                        }
                    )
                ]
            );

            // INSERT A USER DEFINED TYPE AND CHILD VALUES USING CLEANER SYNTAX

            response = sytelineAPI.InsertRecords(
                idoName: "UserDefinedTypes",
                records: [
                    new(
                        properties: new()
                        {
                           ["Name"] = "MyUserDefinedColor2",
                           ["Description"] = "This is a user defined type."
                        },
                        nestedCollections: [
                            new(
                                idoName: "UserDefinedTypeValues",
                                links: [
                                    new(
                                        parentProperty: "Name",
                                        childProperty: "TypeName"
                                    )
                                ],
                                records: [
                                    new(
                                        properties: new()
                                        {
                                           ["TypeName"] = "MyUserDefinedColor",
                                           ["Value"] = "Red",
                                           ["Description"] = "This is red."
                                        }
                                    ),
                                    new(
                                        properties: new()
                                        {
                                           ["TypeName"] = "MyUserDefinedColor",
                                           ["Value"] = "Green",
                                           ["Description"] = "This is green."
                                        }
                                    )
                                ]
                            )
                        ]
                    )
                ]
            );

        }

    }

}