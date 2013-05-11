/*Copyright (c) 2013, salesforce.com, inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without 
modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, 
this list of conditions and the following disclaimer. Redistributions in 
binary form must reproduce the above copyright notice, this list of 
conditions and the following disclaimer in the documentation and/or other 
materials provided with the distribution. 

Neither the name of salesforce.com, inc. nor the names of its contributors 
may be used to endorse or promote products derived from this software 
without specific prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;

using enterprise = SoapCaller.SfdcReference;
using partner = SoapCaller.SfdcPartnerReference;


namespace SoapCaller
{

    /// <summary>
    ///
    /// Title: sforce Login Sample
    /// Description: Console application illustrating login, session management and server redirection.
    /// Copyright: Copyright (c) 2003
    /// Company: salesforce.com
    /// @author Dave Carroll
    /// @version 2.0
    /// 
    /// </summary>
    class Program
    {

        //private SoapCaller.SfdcReference.SoapClient binding = null; - -can't work out what SforceService references 
        private SoapCaller.SfdcReference.SoapClient binding = null; // I think dot.net 2.0 had a soaphttpclient. with .Url param / and web references
        private static string sessionId = string.Empty;
        private static string serverUrl = string.Empty;

        private partner.LoginResult loginResult = null;
        private String un = "";
        private String pw = "";
        private String token = "";
        private bool loggedIn = false;
        private partner.GetUserInfoResult userInfo = null;
        private String[] accounts = null;
        private String[] contacts = null;
        private String[] tasks = null;
        private DateTime serverTime;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Program program = new Program();
            program.run();
        }

        private void run()
        {
            //setup the menu system
            showMenu();
            string choice = Console.ReadLine();
            //go into a loop waiting for 99 to exit
            while (choice != null && !choice.Equals("99"))
            {
                //proccess the menue request
                try
                {
                    if (choice.Length == 1 || choice.Length == 2)
                    {
                        switch (Convert.ToInt16(choice))
                        {
                            case 1:
                                if (login())
                                {
                                    Console.Write("\nSUCESSFUL LOGIN! Hit the enter key to continue.");
                                    Console.ReadLine();
                                }
                                break;
                            case 2:
                                createAccountSample();
                                break;
                            case 3:
                                createContactSample();
                                break;
                            case 4:
                                createTaskSample();
                                break;
                            case 5:
                                querySample();
                                break;
                            case 6:
                                queryAllSample();
                                break;
                            case 7:
                                retrieveSample();
                                break;
                            case 8:
                                updateAccountSample();
                                break;
                            case 9:
                                upsertSample();
                                break;
                            case 10:
                                mergeSample();
                                break;
                            case 11:
                                convertLeadSample();
                                break;
                            case 12:
                                deleteSample();
                                break;
                            case 13:
                                undeleteSample();
                                break;
                            case 14:
                                processSample();
                                break;
                            case 15:
                                describeGlobalSample();
                                break;
                            case 16:
                                describeSample();
                                break;
                            case 17:
                                describeSObjectsSample();
                                break;
                            case 18:
                                describeTabsSample();
                                break;
                            case 19:
                                describeLayoutSample();
                                break;
                            case 20:
                                describeSoftphoneLayoutSample();
                                break;
                            case 21:
                                setPasswordSample();
                                break;
                            case 22:
                                resetPasswordSample();
                                break;
                            case 23:
                                searchSample();
                                break;
                            case 24:
                                getDeletedSample();
                                break;
                            case 25:
                                getUpdatedSample();
                                break;
                            case 26:
                                getServerTimestampSample();
                                break;
                            case 27:
                                getUserInfoSample();
                                break;
                            case 28:
                                emptyRecycleBinSample();
                                break;
                            case 29:
                                sendEmailSample();
                                break;
                            case 30:
                                searchFilterSample();
                                break;
                            case 31:
                                createCustomObjectSample();
                                break;
                            case 32:
                                createCustomFieldSample();
                                break;
                        }
                    }
                    showMenu();

                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    Console.Write("Enter the number of the sample to run: ");
                }
                choice = Console.ReadLine();
            }
        }

        private void showMenu()
        {
            Console.WriteLine(" 1. Login Only");
            Console.WriteLine(" 2. Create Account");
            Console.WriteLine(" 3. Create Contact");
            Console.WriteLine(" 4. Create Task");
            Console.WriteLine(" 5. Query");
            Console.WriteLine(" 6. Query All");
            Console.WriteLine(" 7. Retrieve");
            Console.WriteLine(" 8. Update");
            Console.WriteLine(" 9. Upsert Account");
            Console.WriteLine("10. Merge Accounts");
            Console.WriteLine("11. Convert Lead");
            Console.WriteLine("12. Delete");
            Console.WriteLine("13. Un-delete");
            Console.WriteLine("14. Process Workflow");
            Console.WriteLine("15. Describe Global");
            Console.WriteLine("16. Describe SObject");
            Console.WriteLine("17. Describe Mulitple SObjects");
            Console.WriteLine("18. Describe Tabs");
            Console.WriteLine("19. Describe Layout");
            Console.WriteLine("20. Describe Softphone Layout");
            Console.WriteLine("21. Set Password");
            Console.WriteLine("22. Reset Password");
            Console.WriteLine("23. Search Sample");
            Console.WriteLine("24. Get Deleted");
            Console.WriteLine("25. Get Updated");
            Console.WriteLine("26. Get Server Timestamp");
            Console.WriteLine("27. Get User Info");
            Console.WriteLine("28. Empty Recycle Bin");
            Console.WriteLine("29. Send Email");
            Console.WriteLine("30. Search Filter");
            Console.WriteLine("31. Create/Delete Custom Object");
            Console.WriteLine("32. Create Custom Field");
            Console.WriteLine(" ");
            Console.WriteLine("99. Exit");
            Console.WriteLine(" ");
            //if (binding != null) Console.WriteLine("(" + binding.Url + ")");
            Console.Write("Enter the number of the sample to run: ");
        }


        /* 
         * login sample
         * Prompts for username and password, set class variable binding
         * resets the url for the binding and adds the session header to 
         * the binding class variable
         */
        private bool login()
        {
            Console.Write("Enter user name: ");
            un = Console.ReadLine();
            if (un == null)
            {
                return false;
            }
            Console.Write("Enter password: ");
            pw = Console.ReadLine();
            if (pw == null)
            {
                return false;
            }

            Console.Write("Enter Token: ");
            token = Console.ReadLine();
            if (token == null)
            {
                return false;
            }

            //Provide feed back while we create the web service binding
            Console.WriteLine("Creating the binding to the web service...");

            /*
                * Create the binding to the sforce servics
                */
            binding = new SoapCaller.SfdcReference.SoapClient();

            // Time out after a minute
            //binding.Timeout = 60000;

            //Attempt the login giving the user feedback
            Console.WriteLine("LOGGING IN NOW....");
            //binding.Proxy = new System.Net.WebProxy("localhost:8082");

            try
            {

                Console.Write("Enter user name: ");
                un = Console.ReadLine();
                if (un == null)
                {
                    return false;
                }
                Console.Write("Enter password: ");
                pw = Console.ReadLine();
                if (pw == null)
                {
                    return false;
                }
                Console.Write("Enter Token: ");
                token = Console.ReadLine();
                if (token == null)
                {
                    return false;
                }

                //Provide feed back while we create the web service binding
                Console.WriteLine("Creating the binding to the web service...");



                //login to the SOAP Enterprise endpoint
                AuthenticateSfdcEnterpriseUser(un,pw,token);

                //login to the SOAP Partner endpoint
                AuthenticateSfdcPartnerUser(un, pw, token); ;

         
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                // This is likley to be caused by bad username or password
                Console.Write(e.Message + ", please try again.\n\nHit return to continue...");
                Console.ReadLine();
                return false;
            }
            catch (Exception e)
            {
                // This is something else, probably comminication
                Console.Write(e.Message + ", please try again.\n\nHit return to continue...");
                Console.ReadLine();
                return false;
            }

            Console.WriteLine("\nThe session id is: " + loginResult.sessionId);
            Console.WriteLine("\nThe new server url is: " + loginResult.serverUrl);


            //Change the binding to the new endpoint
            serverUrl = loginResult.serverUrl;

            //Create a new session header object and set the session id to that returned by the login
            sessionId  = loginResult.sessionId;

            loggedIn = true;

            // call the getServerTimestamp method
            getServerTimestampSample();

            ///call the getUserInfo method
            getUserInfoSample();

            return true;

        }

        private void getServerTimestampSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                Console.WriteLine("\nGetting server's timestamp...");
                //call the getServerTimestamp method
               // partner.GetServerTimestampResult gstr = binding.getServerTimestamp();
                //serverTime = gstr.timestamp;
                //access the return properties
             //   Console.WriteLine(gstr.timestamp.ToLongDateString() + " " + gstr.timestamp.ToLongTimeString());
            }
            catch (Exception ex2)
            {
                Console.WriteLine("ERROR: getting server timestamp.\n" + ex2.Message);
            }

        }

        private void getUserInfoSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                Console.WriteLine("\nGetting user info... n/a");
                //call the getUserInfo method
                /*userInfo = binding.getUserInfo();
                //access the return properties
                Console.WriteLine("User Name: " + userInfo.userFullName);
                Console.WriteLine("User Email: " + userInfo.userEmail);
                Console.WriteLine("User Language: " + userInfo.userLanguage);
                Console.WriteLine("User Organization: " + userInfo.organizationName);*/
            }
            catch (Exception ex3)
            {
                Console.WriteLine("ERROR: getting user info.\n" + ex3.Message);
            }
        }


        private void emptyRecycleBinSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                Console.Write("\nEmpty the recycle bin? (Y/N) ");
                String response = Console.ReadLine();
                if (response.ToLower().Equals("y"))
                {
                    // We will create some accounts and then delete them and use
                    // the ids for the emptyRecycleBin method.
                    partner.sObject[] accounts = new partner.sObject[10];
                    for (int i = 0; i < accounts.Length; i++)
                    {
                        accounts[i] = new partner.sObject();
                        accounts[i].Any = new System.Xml.XmlElement[1];
                        accounts[i].Any[0] = GetNewXmlElement("Name", "Test account " + i);
                        accounts[i].type = "Account";
                    }
                    // save the new accounts
                    String[] createdItemIds = new String[10];
                    partner.SaveResult[] sr = binding.create(accounts);
                    for (int i = 0; i < sr.Length; i++)
                    {
                        if (sr[i].success)
                        {
                            createdItemIds[i] = sr[i].id;
                        }
                    }
                    // now, delete them
                    partner.DeleteResult[] dr = binding.delete(createdItemIds);
                    String[] deletedItemIds = new String[sr.Length];
                    for (int i = 0; i < dr.Length; i++)
                    {
                        if (dr[i].success)
                        {
                            deletedItemIds[i] = dr[i].id;
                        }
                    }

                    //call the emptyRecycleBin method
                    partner.EmptyRecycleBinResult[] results = binding.emptyRecycleBin(deletedItemIds);

                    for (int i = 0; i < results.Length; i++)
                    {
                        if (results[i].success)
                        {
                            Console.WriteLine("Lead with id: " + results[i].id + " was successfully queued for permanant delete.");
                        }
                        else
                        {
                            Console.WriteLine("A problem occurred queueing item with id: " + results[i].id + " for permanent delete.");
                        }
                    }
                    Console.WriteLine("Empty recycle bin finished.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex3)
            {
                Console.WriteLine("ERROR: emptying recycle bin.\n" + ex3.Message);
            }
        }

        private void sendEmailSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                Console.Write("Enter a valid email address for the bcc: ");
                String bccAddress = Console.ReadLine();
                partner.SingleEmailMessage[] messages = new partner.SingleEmailMessage[1];
                messages[0] = new partner.SingleEmailMessage();
                messages[0].bccAddresses = bccAddress.Length > 0 ? new String[] { bccAddress } : null;
                messages[0].bccSender = true;
                Console.Write("Enter a valid email address for the cc: ");
                String ccAddress = Console.ReadLine();
                messages[0].ccAddresses = ccAddress.Length > 0 ? new String[] { ccAddress } : null;
                messages[0].emailPriority = partner.EmailPriority.Normal;
                messages[0].htmlBody = "<b>This is a message</b><br>from the api sample.";
                //The following field is only valid if the targetObjectId was used.
                //messages[0].saveAsActivity = saveAsActivity;
                messages[0].subject = "Test Message From Sample";
                messages[0].useSignature = false;
                messages[0].plainTextBody = "This is a message from the api";
                Console.Write("Enter a valid email address for the sender: ");
                String replyTo = Console.ReadLine();
                Console.Write("Enter a valid email address for the recipient of this email: ");
                String toAddress = Console.ReadLine();
                if (toAddress.Length > 0 && replyTo.Length > 0)
                {
                    messages[0].replyTo = replyTo;
                    messages[0].toAddresses = new String[] { toAddress };
                    //The next line is used if you have an id for a user, contact or
                    //lead that is the recipient.  If this is the case, you don't need
                    //to set the toAddresses field.
                    //messages[0].targetObjectId = "003000000fexGGH";
                    partner.SendEmailResult[] result = binding.sendEmail(messages);
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (result[i].success)
                        {
                            Console.Write("Successfully sent the email to " + toAddress);
                        }
                        else
                        {
                            Console.Write("Error from the platform: " + result[i].errors[0].message);
                        }
                    }
                    Console.ReadLine();
                }
                else
                {
                    Console.Write("No to address entered, so we are bailing.  Hit return to continue.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex3)
            {
                Console.WriteLine("ERROR: sending email.\n" + ex3.Message);
            }
        }
        
        private System.Xml.XmlElement GetNewXmlElement(string Name, string nodeValue)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlElement xmlel = doc.CreateElement(Name);
            xmlel.InnerText = nodeValue;
            return xmlel;
        }

        private void updateAccountSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                //check to see if there are any accounts created in this session
                if (accounts == null)
                {
                    Console.WriteLine("\nUpdate operation not completed.  You will need to create an account during this session to run the update sample.");
                    Console.Write("\nHit return to continue...");
                    Console.ReadLine();
                    return;
                }

                //create the account object to hold our changes
                partner.sObject updateAccount = new partner.sObject();
                //need to have the id so that web service knows which account to update
                updateAccount.Id = accounts[0];
                //set a new value for the name property
                updateAccount.Any = new System.Xml.XmlElement[] { this.GetNewXmlElement("Name", "New Account Name from Update Sample") };
                updateAccount.type = "Account";

                //create one that will throw an error
                partner.sObject errorAccount = new partner.sObject();
                errorAccount.Id = "S:DLFKJLFKJ";
                errorAccount.Any = new System.Xml.XmlElement[] { this.GetNewXmlElement("Name", "Error") };
                errorAccount.fieldsToNull = new string[] { "Name" };
                errorAccount.type = "Account";

                //call the update passing an array of object
                partner.SaveResult[] saveResults = binding.update(new partner.sObject[] { updateAccount, errorAccount });

                //loop through the results, checking for errors
                for (int j = 0; j < saveResults.Length; j++)
                {
                    Console.WriteLine("Item: " + j);
                    if (saveResults[j].success)
                        Console.WriteLine("An account with an id of: " + saveResults[j].id + " was updated.\n");
                    else
                    {
                        Console.WriteLine("Item " + j.ToString() + " had an error updating.");
                        Console.WriteLine("    The error reported was: " + saveResults[j].errors[0].message + "\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to succesfully update an account, error message was: \n"
                           + ex.Message);
            }
            Console.WriteLine("\nHit return to continue...");
            Console.ReadLine();
        }


        #region 'Winter 07'
        private void describeSoftphoneLayoutSample()
        {
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            try
            {
                partner.DescribeSoftphoneLayoutResult slr = binding.describeSoftphoneLayout();
                partner.DescribeSoftphoneLayoutCallType[] callTypes = slr.callTypes;
                Console.WriteLine("There are " + callTypes.Length
                        + " call types.");
                for (int i = 0; i < callTypes.Length; i++)
                {
                    partner.DescribeSoftphoneLayoutCallType callType = callTypes[i];
                    partner.DescribeSoftphoneLayoutInfoField[] fields = callType.infoFields;
                    Console.WriteLine("    There are "
                            + fields.Length
                            + " info fields.");
                    for (int j = 0; j < fields.Length; j++)
                    {
                        Console.WriteLine("\t" + fields[j].name + "\n");
                    }
                    partner.DescribeSoftphoneLayoutSection[] sections = callType.sections;
                    Console.WriteLine("\tThere are " + sections.Length + " sections on this layout.");
                    for (int j = 0; j < sections.Length; j++)
                    {
                        partner.DescribeSoftphoneLayoutSection section = sections[j];
                        partner.DescribeSoftphoneLayoutItem[] items = section.items;
                        for (int k = 0; k < items.Length; k++)
                        {
                            partner.DescribeSoftphoneLayoutItem item = items[k];
                            Console.WriteLine("Section " + j + " - item api name: " + item.itemApiName);
                        }
                    }
                }
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
            catch (System.Web.Services.Protocols.SoapException af)
            {
                Console.WriteLine("\nFailed to describe softphone layout, error message was: \n" + af.Message + "\nHit return to continue...");
                Console.ReadLine();
            }
        }
        private void queryAllSample()
        {
            // Verify that we are already authenticated, if not
            // call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            //For this sample we will create an account and then delete it
            //to demonstrate the power of queryAll
            Console.Write("\nEnter a name for a test account: ");
            String accountName = Console.ReadLine();
            if (accountName == null || accountName.Length == 0)
            {
                return;
            }
            createAndDeleteAnAccount(accountName);

            //Now for Query All.  Query all allows you to return items that have been moved to the recycle
            //bin, like the account we just deleted.
            partner.QueryResult qr = null;
            try
            {
                qr = binding.queryAll("select id, Name from Account where name = '" + accountName + "' and IsDeleted = true");
                if (qr.size != 0)
                {
                    partner.sObject account = qr.records[0];
                    Console.WriteLine("Retrieved the deleted account: " + getFieldValue("Name", account.Any));
                }
                else
                {
                    Console.WriteLine("Hmm...\nDid not find the account, that's strange.");
                }
                Console.WriteLine("\nQuery succesfully executed.\nHit return to continue...");
                Console.ReadLine();
            }
            catch (System.Web.Services.Protocols.SoapException af)
            {
                Console.WriteLine("\nFailed to execute query succesfully, error message was: \n" + af.Message + "\nHit return to continue...");
                Console.ReadLine();
            }
        }
        private void processSample()
        {
            // Verify that we are already authenticated, if not
            // call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            try
            {
                //First step is to create an account that matches the approval process criteria
                partner.sObject acct = new partner.sObject();
                acct.type = "Account";
                acct.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Name", "API Approval Sample") };
                acct.Id = binding.create(new partner.sObject[] { acct })[0].id;

                // Next step is to submit the account for approval using a ProcessSubmitRequest
                partner.ProcessSubmitRequest psr = new partner.ProcessSubmitRequest();
                psr.objectId = acct.Id;
                psr.comments = "This approval request was initiated from the API.";
                partner.ProcessResult p_res = binding.process(new partner.ProcessRequest[] { psr })[0];
                if (p_res.success)
                {
                    //Since the submission was successful we can now approve or reject it with 
                    // a ProcessWorkItmeRequest
                    partner.ProcessWorkitemRequest pwr = new partner.ProcessWorkitemRequest();
                    pwr.action = "Approve";
                    pwr.comments = "This request was approved from the API.";
                    pwr.workitemId = p_res.newWorkitemIds[0];
                    p_res = binding.process(new partner.ProcessRequest[] { pwr })[0];
                    if (p_res.success)
                    {
                        Console.WriteLine("Successfully submitted and then approved an approval request.");
                    }
                    else
                    {
                        Console.WriteLine("Error approving the work item: " + p_res.errors[0].message);
                    }
                }
                else
                {
                    Console.WriteLine("Error submitting the account for approval: " + p_res.errors[0].message);
                }
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("The API returned a fault: " + e.Message);
            }
            Console.WriteLine("\nHit the enter key to continue...");
            Console.ReadLine();
        }
        private String createAndDeleteAnAccount(String accountName)
        {
            String returnId = null;
            partner.sObject acct = new partner.sObject();
            acct.type = "Account";
            acct.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Name", (accountName == null) ? "QueryAll Sample" : accountName) };
            try
            {
                //We are only creating one account so we can index the return array directly
                partner.SaveResult sr = binding.create(new partner.sObject[] { acct })[0];
                if (sr.success)
                {


                    acct.Id = sr.id;
                    //Ok, now we will delete that account
                    partner.DeleteResult dr = binding.delete(new String[] { acct.Id })[0];
                    if (!dr.success)
                    {
                        Console.WriteLine("The web service would not let us delete the account: \n" + dr.errors[0].message + "\nHit return to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        returnId = acct.Id;
                    }


                }
                else
                {
                    Console.WriteLine("The web service would not let us create the account: \n" + sr.errors[0].message + "\nHit return to continue...");
                    Console.ReadLine();
                }
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("Error creating test account: " + e.Message + "\nHit return to continue...");
                Console.ReadLine();
            }
            return returnId;
        }
        private void undeleteSample()
        {
            //Create and delete the account, like the queryAll sample
            //Undelete the account

            // Verify that we are already authenticated, if not
            // call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            //For this sample we will create an account and then delete it
            //to demonstrate the power of queryAll
            Console.Write("\nEnter a name for a test account:");
            String accountName = Console.ReadLine();
            if (accountName == null || accountName.Length == 0)
            {
                return;
            }
            String accountId = createAndDeleteAnAccount(accountName);
            Console.WriteLine("You can check your recycle bin now to see the deleted account.\nHit return to continue...");
            Console.ReadLine();

            //Now, we can undelete the account
            try
            {
                partner.UndeleteResult udr = binding.undelete(new String[] { accountId })[0];
                if (udr.success)
                {
                    Console.WriteLine("The account was successfully undeleted.");
                    Console.WriteLine("If you check your recycle bin you will see the account is no longer present.");
                }
                else
                {
                    Console.WriteLine("Undelete failed: " + udr.errors[0].message);
                }
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("Error un-deleting test account: " + e.Message);
            }
            Console.WriteLine("\nHit return to continue...");
            Console.ReadLine();
        }
        private void mergeSample()
        {
            // TODO Auto-generated method stub
            // Verify that we are already authenticated, if not
            // call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            try
            {
                partner.sObject masterAccount = new partner.sObject();
                masterAccount.type = "Account";
                masterAccount.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Name", "MasterAccount") };
                partner.SaveResult masterAccountSaveResult;
                masterAccountSaveResult = binding.create(new partner.sObject[] { masterAccount })[0];
                masterAccount.Id = masterAccountSaveResult.id;
                System.Xml.XmlElement[] masterAny = masterAccount.Any;
                System.Array.Resize(ref masterAny, (masterAccount.Any.Length + 1));
                masterAny[masterAny.Length - 1] = GetNewXmlElement("Description", "Old description");
                masterAccount.Any = masterAny;

                partner.sObject accountToMerge = new partner.sObject();
                accountToMerge.type = "Account";
                accountToMerge.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Name", "AccountToMerge"), 
                    GetNewXmlElement("Description", "Duplicate account") };

                partner.SaveResult accountToMergeSaveResult = binding.create(new partner.sObject[] { accountToMerge })[0];

                //Attach a note, which will get re-parented
                partner.sObject note = new partner.sObject();
                note.type = "Note";
                note.Any = new System.Xml.XmlElement[] { GetNewXmlElement("ParentId", accountToMergeSaveResult.id), 
                    GetNewXmlElement("Body", "This note will be moved to the MasterAccount during merge"), 
                    GetNewXmlElement("Title", "Test note to be reparented.") };

                partner.SaveResult noteSave = binding.create(new partner.sObject[] { note })[0];

                partner.MergeRequest mr = new partner.MergeRequest();

                //Perform an update on the master record as part of the merge:
                masterAccount.Any[masterAccount.Any.Length - 1] = GetNewXmlElement("Description", "Was merged");

                mr.masterRecord = masterAccount;
                mr.recordToMergeIds = (new String[] { accountToMergeSaveResult.id });
                partner.MergeResult result = binding.merge(new partner.MergeRequest[] { mr })[0];

                Console.WriteLine("Merged " + result.success + " got " +
                result.updatedRelatedIds.Length + " updated child records\nHit return to continue.");
                Console.ReadLine();

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("Error merging account: " + e.Message + "\nHit return to continue...");
                Console.ReadLine();
            }
        }
        private System.Collections.Hashtable makeFieldMap(partner.Field[] fields)
        {
            System.Collections.Hashtable fieldMap = new System.Collections.Hashtable();
            for (int i = 0; i < fields.Length; i++)
            {
                partner.Field field = fields[i];
                fieldMap.Add(field.name, field);
            }
            return fieldMap;
        }

        private void upsertSample()
        {
            // TODO Auto-generated method stub
            // Verify that we are already authenticated, if not
            // call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            try
            {

                partner.DescribeSObjectResult dsr = binding.describeSObject("Account");
                System.Collections.Hashtable fieldMap = this.makeFieldMap(dsr.fields);
                if (!fieldMap.ContainsKey("External_Id__c"))
                {
                    Console.WriteLine("\n\nNOTICE: To run the upsert sample you need \nto add a custom field named \nExternal_Id to the account object.  \nSet the size of the field to 8 characters \nand be sure to check the 'External Id' checkbox.");
                } else {
                    //First, we need to make sure the test accounts do not exist.
                    partner.QueryResult qr = binding.query("Select Id From Account Where External_Id__c = '11111111' or External_Id__c = '22222222'");
                    if (qr.size > 0)
                    {
                        partner.sObject[] accounts = (partner.sObject[])qr.records;
                        //Get the ids
                        String[] ids = new String[accounts.Length];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            ids[i] = accounts[i].Id;
                        }
                        //Delete the accounts
                        binding.delete(ids);
                    }

                    //Create a new account using create, we wil use this to update via upsert
                    //We will set the external id to be ones so that we can use that value for the upsert
                    partner.sObject newAccount = new partner.sObject();
                    newAccount.type = "Account";
                    newAccount.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Name", "Account to update"), 
                        GetNewXmlElement("External_Id__c", "11111111") };
                    binding.create(new partner.sObject[] { newAccount });

                    //Now we will create an account that should be updated on insert based
                    //on the external id field.
                    partner.sObject updateAccount = new partner.sObject();
                    updateAccount.type = "Account";
                    updateAccount.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Website", "http://www.website.com"), 
                        GetNewXmlElement("External_Id__c", "11111111") };

                    // This account is meant to be new
                    partner.sObject createAccount = new partner.sObject();
                    createAccount.type = "Account";
                    createAccount.Any = new System.Xml.XmlElement[] { GetNewXmlElement("Name", "My Company, Inc"), 
                        GetNewXmlElement("External_Id__c", "22222222") };

                    //We have our two accounts, one should be new, the other should be updated.
                    try
                    {
                        // Invoke the upsert call and save the results.
                        // Use External_Id custom field for matching records
                        partner.UpsertResult[] upsertResults = binding.upsert("External_Id__c", new partner.sObject[] { createAccount, updateAccount });
                        for (int i = 0; i < upsertResults.Length; i++)
                        {
                            partner.UpsertResult result = upsertResults[i];
                            if (result.success)
                            {
                                Console.WriteLine("\nUpsert succeeded.");
                                Console.WriteLine((result.created ? "Inserted" : "Updated") + " account, id is " + result.id);
                            }
                            else
                            {
                                Console.WriteLine("The Upsert failed because: " + result.errors[0].message);
                            }
                        }
                    }
                    catch (System.Web.Services.Protocols.SoapException ex)
                    {
                        Console.WriteLine("Error from web service: " + ex.Message);
                    }
                }
                Console.WriteLine("\nPress the RETURN key to continue...");
                Console.ReadLine();
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                Console.WriteLine("Error merging account: " + ex.Message + "\nHit return to continue...");
                Console.ReadLine();
            }
        }
        #endregion

        private void createLeadSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {

                partner.sObject[] leads = new partner.sObject[1];
                partner.sObject lead;
                lead = new partner.sObject();
                int index = 0;
                System.Xml.XmlElement[] leadEls = new System.Xml.XmlElement[17];
                leadEls[index++] = GetNewXmlElement("AnnualRevenue", System.Xml.XmlConvert.ToString(1000000.0));
                leadEls[index++] = GetNewXmlElement("AnnualRevenueSpecified", System.Xml.XmlConvert.ToString(true));
                leadEls[index++] = GetNewXmlElement("City", "San Francisco");
                leadEls[index++] = GetNewXmlElement("Company", "Acme Anvils");
                leadEls[index++] = GetNewXmlElement("Country", "United States");
                leadEls[index++] = GetNewXmlElement("CurrencyIsoCode", "USD");
                leadEls[index++] = GetNewXmlElement("Description", "This is a lead that can be converted.");
                leadEls[index++] = GetNewXmlElement("DoNotCall", System.Xml.XmlConvert.ToString(false));
                leadEls[index++] = GetNewXmlElement("DoNotCallSpecified", System.Xml.XmlConvert.ToString(true));
                Console.Write("\nPlease enter an email for the lead we are creating.");
                string email = Console.ReadLine();
                if (email == null)
                    leadEls[index++] = GetNewXmlElement("Email", "some.email@some.domain.com");
                else
                    leadEls[index++] = GetNewXmlElement("Email", email);
                leadEls[index++] = GetNewXmlElement("Fax", "5555555555");
                leadEls[index++] = GetNewXmlElement("FirstName", "Wiley");
                leadEls[index++] = GetNewXmlElement("HasOptedOutOfEmail", System.Xml.XmlConvert.ToString(false));
                leadEls[index++] = GetNewXmlElement("HasOptedOutOfEmailSpecified", System.Xml.XmlConvert.ToString(true));
                leadEls[index++] = GetNewXmlElement("Industry", "Blacksmithery");
                leadEls[index++] = GetNewXmlElement("LastName", "Coyote");
                leadEls[index++] = GetNewXmlElement("LeadSource", "Web");
                leadEls[index++] = GetNewXmlElement("MobilePhone", "4444444444");
                leadEls[index++] = GetNewXmlElement("NumberOfEmployees", System.Xml.XmlConvert.ToString(30));
                leadEls[index++] = GetNewXmlElement("NumberOfEmployeesSpecified", System.Xml.XmlConvert.ToString(true));
                leadEls[index++] = GetNewXmlElement("NumberofLocations__c", System.Xml.XmlConvert.ToString(1.0));
                leadEls[index++] = GetNewXmlElement("NumberofLocations__cSpecified", System.Xml.XmlConvert.ToString(true));
                leadEls[index++] = GetNewXmlElement("Phone", "6666666666");
                leadEls[index++] = GetNewXmlElement("PostalCode", "94105");
                leadEls[index++] = GetNewXmlElement("Rating", "Hot");
                leadEls[index++] = GetNewXmlElement("Salutation", "Mr.");
                leadEls[index++] = GetNewXmlElement("State", "California");
                leadEls[index++] = GetNewXmlElement("Status", "Working");
                leadEls[index++] = GetNewXmlElement("Street", "10 Downing Street");
                leadEls[index++] = GetNewXmlElement("Title", "Director of Directors");
                leadEls[index++] = GetNewXmlElement("Website", "www.acmeanvils.com");

                lead.Any = leadEls;
                lead.type = "Lead";
                leads[0] = lead;

                partner.SaveResult[] sr = binding.create(leads);
                for (int j = 0; j < sr.Length; j++)
                {
                    if (sr[j].success)
                    {
                        Console.WriteLine("A lead was create with an id of: "
                            + sr[j].id);
                    }
                    else
                    {
                        //there were errors during the create call, go through the errors
                        //array and write them to the screen
                        for (int i = 0; i < sr[j].errors.Length; i++)
                        {
                            //get the next error
                            partner.Error err = sr[j].errors[i];
                            Console.WriteLine("Errors were found on item " + j.ToString());
                            Console.WriteLine("Error code is: " + err.statusCode.ToString());
                            Console.WriteLine("Error message: " + err.message);
                        }
                    }
                    Console.Write("\nHit return to continue...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to execute query succesfully, error message was: \n"
                    + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private partner.sObject getUnconvertedLead()
        {
            //get a list of leads so the user can select one 
            partner.QueryResult qr = binding.query("Select Id, FirstName, LastName from Lead where ConvertedDate = Null");
            if (qr.size == 0)
            {	//No leads where found that have not been converted, so....
                //we will create a lead and then run the query again
                Console.Write("No unconverted leads found, will create one for you...");
                this.createLeadSample();
                qr = binding.query("Select Id, FirstName, LastName from Lead where ConvertedDate = Null");
                if (qr.size == 0)
                    return null;
            }
            for (int i = 0; i < qr.records.Length; i++)
            {
                partner.sObject lead = qr.records[i];
                Console.WriteLine((i + 1) + ": " + this.getFieldValue("FirstName", lead.Any) + " " + getFieldValue("LastName", lead.Any));
            }
            Console.Write("/nSelect the number of the lead to convert. ");
            string selectedLead = Console.ReadLine();
            if (selectedLead == null)
                return null;
            else
            {
                try
                {
                    return qr.records[Convert.ToInt16(selectedLead) - 1];
                }
                catch
                {
                    Console.WriteLine("The number you selected is not valid, exiting...");
                    return null;
                }
            }
        }

        private partner.sObject getContact()
        {
            //get a list of contacts so the user can select one 
            partner.QueryResult qr = binding.query("Select Id, FirstName, LastName, AccountId from Contact Where not AccountId = null");
            if (qr.size == 0)
            {	//No leads where found that have not been converted, so....
                //we will create a lead and then run the query again
                Console.Write("No contacts found, will create one for you...");
                this.createContactSample();
                qr = binding.query("Select Id, FirstName, LastName, AccountId from Contact");
                if (qr.size == 0)
                    return null;
            }
            for (int i = 0; i < qr.records.Length; i++)
            {
                partner.sObject contact = qr.records[i];
                Console.WriteLine((i + 1) + ": " + getFieldValue("FirstName", contact.Any) + " " + getFieldValue("LastName", contact.Any));
            }
            Console.Write("/nSelect the number of the contact to use. ");
            string selectedContact = Console.ReadLine();
            if (selectedContact == null)
                return null;
            else
            {
                try
                {
                    return qr.records[Convert.ToInt16(selectedContact) - 1];
                }
                catch
                {
                    Console.WriteLine("The number you selected is not valid, exiting...");
                    return null;
                }
            }
        }

        private partner.sObject getAccount()
        {
            //get a list of Accounts so the user can select one
            partner.QueryResult qr = binding.query("Select Id, Name from Account");
            if (qr.size == 0)
            {	//No accounts found (Not Likely), so...
                //we will create an account and then run the query again
                this.createAccountSample();
                qr = binding.query("Select Id, Name from Account");
                if (qr.size == 0) return null;
            }
            for (int i = 0; i < qr.records.Length; i++)
            {
                partner.sObject account = qr.records[i];
                Console.WriteLine((i + 1) + ": " + getFieldValue("Name", account.Any));
            }
            Console.Write("/nSelect the number of the account to use. ");
            string selectedAccount = Console.ReadLine();
            if (selectedAccount == null)
                return null;
            else
            {
                try
                {
                    return qr.records[Convert.ToInt16(selectedAccount) - 1];
                }
                catch
                {
                    Console.WriteLine("The number you selected is not valid, exiting...");
                    return null;
                }
            }
        }

        private string getAccountName(string accountId)
        {
            partner.sObject[] ret = binding.retrieve("Name", "Account", new string[] { accountId });
            if (ret != null)
            {
                return getFieldValue("Name", ret[0].Any);
            }
            else
            {
                return null;
            }
        }

        private string getLeadStatus()
        {
            partner.QueryResult qr = binding.query("Select Id, MasterLabel from LeadStatus Where IsConverted = true");
            if (qr.size > 0)
            {
                Console.WriteLine("\n");
                for (int i = 0; i < qr.records.Length; i++)
                {
                    Console.WriteLine((i + 1) + ": " + getFieldValue("MasterLabel", qr.records[i].Any));
                }
                Console.Write("\nEnter the number of the status to use. ");
                string stat = Console.ReadLine();
                if (stat != null)
                    return getFieldValue("MasterLabel", qr.records[Convert.ToInt16(stat) - 1].Any);
                else
                    return getFieldValue("MasterLabel", qr.records[0].Any);
            }
            else
            {
                return null;
            }

        }


        private void convertLeadSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            //This is the structure we need to fill in to effect a conversion
            partner.LeadConvert lc = new partner.LeadConvert();
            string summary = "Converting lead to contact.\n";

            //When converting a lead you have the option of creating a new contact, or
            //merging the lead with an existing contact.  This sample will show both
            //cases.  So the first thing to do, is get the lead to convert.

            partner.sObject lead = this.getUnconvertedLead();
            if (lead == null)
            {
                Console.WriteLine("No lead was selected, conversion failed.");
                return;
            }
            //Set the lead id on the structure.
            lc.leadId = lead.Id.Substring(0, 15);


            //Now we need to determine if the user wants to merge the lead to an existing contact,
            //or have a new contact created.
            Console.Write("/nDo you want to merge this lead into an existing contact (Y/N)? ");
            string merge = Console.ReadLine();
            partner.sObject contact = null;
            if (merge != null && merge.ToLower().Equals("y"))
            {
                contact = getContact();
                if (contact == null)
                {
                    summary += "  Create new contact.\n";
                }
                else
                {
                    summary += "  Merge with " + getFieldValue("FirstName", contact.Any) + " " + getFieldValue("LastName", contact.Any) + "\n";
                    lc.contactId = contact.Id.Substring(0, 15);
                    //We have the option of resetting the contact status to that of the lead,
                    //so we will query the user for that information.
                    Console.Write("Do you want to overwrite the contact status with the lead status (Y/N)? ");
                    string overWrite = Console.ReadLine();
                    if (overWrite != null && overWrite.ToLower().Equals("y"))
                        lc.overwriteLeadSource = true;
                    else
                        lc.overwriteLeadSource = false;
                }
            }
            else
                summary += "  Create new contact.\n";

            //if a contact is chosen, we will use the accountid of the contact, this must
            //be specified.  If the contact is not chosen, then the user needs to select
            //an account to place the new contact in.
            string accountId = null;

            if (contact != null)
            {
                accountId = getFieldValue("AccountId", contact.Any);
                lc.accountId = accountId.Substring(0, 15);
            }
            else			//get an account chosen by the user
            {
                Console.Write("Do you want to create a new account for this lead (Y/N)? ");
                string newAccount = Console.ReadLine();
                if (newAccount == null || !newAccount.ToLower().Equals("y"))
                    accountId = this.getAccount().Id.Substring(0, 15);

                if (accountId != null)
                {
                    lc.accountId = accountId;
                    string accountName = getAccountName(accountId);
                    if (accountName != null)
                        summary += "  New contact will be in account '" + accountName + "'.\n";
                    else
                        summary += "  New contact will be in account with an ID of '" + accountId + "'.\n";

                }
                else
                {
                    summary += "  A new account will be created.";
                }
            }
            //We will now ask the user if they would like to create a new opportunity
            //based on the information on this lead
            Console.Write("Do you want to create an opportunity for this conversion (Y/N)? ");
            string opp = Console.ReadLine();
            lc.doNotCreateOpportunity = true;
            if (opp != null && opp.ToLower().Equals("y"))
            {
                lc.doNotCreateOpportunity = false;
                Console.Write("Enter the name of the opportunity to create.. ");
                string oppName = Console.ReadLine();
                if (oppName == null)
                {
                    Console.WriteLine("No opportunity name given, NO opportunity will be created.");
                    lc.doNotCreateOpportunity = true;
                    summary += "  No opportunity will be created.\n";
                }
                else
                {
                    lc.opportunityName = oppName;
                    summary += "  An opportunity named: " + oppName + " will be created.\n";
                }
            }

            //The lead needs to have it's status updated to reflect the conversion operation,
            //so we will ask the user to select the status to use
            Console.Write("Select the lead status to use to update the converted lead. ");
            lc.convertedStatus = getLeadStatus();
            summary += "  The converted lead will be assigned a status of " + lc.convertedStatus + ".\n";

            //Finally, we have the option of notifying the owner of the lead that it
            //has been converted.
            Console.Write("Would you like to have an email sent to the owner of the lead after the conversion (Y/N)?");
            string sendMail = Console.ReadLine();
            if (sendMail != null && sendMail.ToLower().Equals("y"))
            {
                lc.sendNotificationEmail = true;
                summary += "  Email notification will be sent.\n";
            }
            else
            {
                lc.sendNotificationEmail = false;
                summary += "  Email notificatino will NOT be sent.\n";
            }

            string cont;
            Console.Write("\n\nDEBUG VALUES\n");
            Console.WriteLine("account id: " + lc.accountId);
            Console.WriteLine("contact id: " + lc.contactId);
            Console.WriteLine("converted status: " + lc.convertedStatus);
            Console.WriteLine("do not create opp: " + lc.doNotCreateOpportunity);
            Console.WriteLine("lead id: " + lc.leadId);
            Console.WriteLine("opp name: " + lc.opportunityName);
            Console.WriteLine("overwrite lead source: " + lc.overwriteLeadSource);
            Console.WriteLine("send email: " + lc.sendNotificationEmail);

            Console.Write(summary + "\n Enter 'Y' to convert the lead.");
            cont = Console.ReadLine();
            if (cont != null && cont.ToLower().Equals("y"))
            {
                partner.LeadConvertResult[] lcr = binding.convertLead(new partner.LeadConvert[] { lc });
                //although we only converted a single lead, we will use a loop to process the
                //results since the return value is an array of LeadConvertResults
                for (int i = 0; i < lcr.Length; i++)
                {
                    if (lcr[i].success)
                    {
                        Console.WriteLine("Conversion of lead " + getFieldValue("FirstName", lead.Any) + " " + getFieldValue("LastName", lead.Any) + " was successful.");
                        if (contact.Id.Equals(lcr[i].contactId))
                            Console.WriteLine("  Contact with ID of '" + lcr[i].contactId + "' was merged.");
                        else
                            Console.WriteLine("  Contact with ID of '" + lcr[i].contactId + "' was created.");
                        if (lcr[i].opportunityId != null)
                            Console.WriteLine("  An opportunity with an ID of '" + lcr[i].opportunityId + "' was created.");
                        else
                            Console.WriteLine("  No opportunity was created.");
                        Console.WriteLine("  The contact was create in the account with an ID of '" + lcr[i].accountId + "'.");
                    }
                    else
                    {
                        Console.WriteLine("One or more errors where encountered during the lead conversion process...\n");
                        for (int j = 0; j < lcr[i].errors.Length; j++)
                        {
                            Console.WriteLine((j + 1) + lcr[0].errors[j].message);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Lead conversion was aborted.");
            }
        }

        private void createAccountSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                partner.sObject account;
                partner.sObject[] accs = new partner.sObject[2];
                for (int j = 0; j < accs.Length; j++)
                {
                    account = new partner.sObject();
                    System.Xml.XmlElement[] acct = new System.Xml.XmlElement[14];
                    int index = 0;
                    if (accounts == null)
                        acct[index++] = GetNewXmlElement("AccountNumber", "0000000");
                    else
                        acct[index++] = GetNewXmlElement("AccountNumber", "000000" + (accounts.Length + 1));
                    //account.setAnnualRevenue(new java.lang.Double(4000000.0));
                    acct[index++] = GetNewXmlElement("BillingCity", "Wichita");
                    acct[index++] = GetNewXmlElement("BillingCountry", "US");
                    acct[index++] = GetNewXmlElement("BillingState", "KA");
                    acct[index++] = GetNewXmlElement("BillingStreet", "4322 Haystack Boulevard");
                    acct[index++] = GetNewXmlElement("BillingPostalCode", "87901");
                    acct[index++] = GetNewXmlElement("Description", "World class hay makers.");
                    acct[index++] = GetNewXmlElement("Fax", "555.555.5555");
                    acct[index++] = GetNewXmlElement("Industry", "Farming");
                    acct[index++] = GetNewXmlElement("Name", "Golden Straw");
                    acct[index++] = GetNewXmlElement("NumberOfEmployees", "40");
                    acct[index++] = GetNewXmlElement("Ownership", "Privately Held");
                    acct[index++] = GetNewXmlElement("Phone", "666.666.6666");
                    acct[index++] = GetNewXmlElement("Website", "www.oz.com");
                    account.type = "Account";
                    account.Any = acct;
                    accs[j] = account;
                }

                //create the object(s) by sending the array to the web service
                partner.SaveResult[] sr = binding.create(accs);
                for (int j = 0; j < sr.Length; j++)
                {
                    if (sr[j].success)
                    {
                        Console.Write(System.Environment.NewLine + "An account was create with an id of: "
                            + sr[j].id);

                        //save the account ids in a class array
                        if (accounts == null)
                        {
                            accounts = new string[] { sr[j].id };
                        }
                        else
                        {
                            string[] tempAccounts = null;
                            tempAccounts = new string[accounts.Length + 1];
                            for (int i = 0; i < accounts.Length; i++)
                                tempAccounts[i] = accounts[i];
                            tempAccounts[accounts.Length] = sr[j].id;
                            accounts = tempAccounts;
                        }
                    }
                    else
                    {
                        //there were errors during the create call, go through the errors
                        //array and write them to the screen
                        for (int i = 0; i < sr[j].errors.Length; i++)
                        {
                            //get the next error
                            partner.Error err = sr[j].errors[i];
                            Console.WriteLine("Errors were found on item " + j.ToString());
                            Console.WriteLine("Error code is: " + err.statusCode.ToString());
                            Console.WriteLine("Error message: " + err.message);
                        }
                    }
                }
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to create account, error message was: \n"
                           + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }

        }

        private void createContactSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {

                partner.sObject[] cons = new partner.sObject[1];
                partner.sObject contact;
                for (int j = 0; j < cons.Length; j++)
                {
                    contact = new partner.sObject();
                    int index = 0;
                    System.Xml.XmlElement[] cont = new System.Xml.XmlElement[17];
                    if (accounts != null)
                    {
                        cont = new System.Xml.XmlElement[cont.Length + 1];
                        cont[index++] = GetNewXmlElement("AccountId", accounts[0]);
                    }
                    cont[index++] = GetNewXmlElement("AssistantName", "Jane");
                    cont[index++] = GetNewXmlElement("AssistantPhone", "777.777.7777");
                    cont[index++] = GetNewXmlElement("Department", "Purchasing");
                    cont[index++] = GetNewXmlElement("Description", "International IT Purchaser");
                    cont[index++] = GetNewXmlElement("Email", "joeblow@isp.com");
                    cont[index++] = GetNewXmlElement("Fax", "555.555.5555");
                    cont[index++] = GetNewXmlElement("MailingCity", "San Mateo");
                    cont[index++] = GetNewXmlElement("MailingCountry", "US");
                    cont[index++] = GetNewXmlElement("MailingState", "CA");
                    cont[index++] = GetNewXmlElement("MailingStreet", "1129 B Street");
                    cont[index++] = GetNewXmlElement("MailingPostalCode", "94105");
                    cont[index++] = GetNewXmlElement("MobilePhone", "888.888.8888");
                    cont[index++] = GetNewXmlElement("FirstName", "Joe");
                    cont[index++] = GetNewXmlElement("LastName", "Blow");
                    cont[index++] = GetNewXmlElement("Salutation", "Mr.");
                    cont[index++] = GetNewXmlElement("Phone", "999.999.9999");
                    cont[index++] = GetNewXmlElement("Title", "Purchasing Director");
                    contact.Any = cont;
                    contact.type = "Contact";
                    cons[j] = contact;
                }
                partner.SaveResult[] sr = binding.create(cons);
                for (int j = 0; j < sr.Length; j++)
                {
                    if (sr[j].success)
                    {
                        Console.WriteLine("A contact was create with an id of: "
                            + sr[j].id);
                        if (accounts != null)
                            Console.WriteLine(" - the contact was associated with the account you created with an id of "
                                + accounts[0]
                                + ".");


                        if (contacts == null)
                        {
                            contacts = new string[] { sr[j].id };
                        }
                        else
                        {
                            string[] tempContacts = null;
                            tempContacts = new string[contacts.Length + 1];
                            for (int i = 0; i < contacts.Length; i++)
                                tempContacts[i] = contacts[i];
                            tempContacts[contacts.Length] = sr[j].id;
                            contacts = tempContacts;
                        }
                    }
                    else
                    {
                        //there were errors during the create call, go through the errors
                        //array and write them to the screen
                        for (int i = 0; i < sr[j].errors.Length; i++)
                        {
                            //get the next error
                            partner.Error err = sr[j].errors[i];
                            Console.WriteLine("Errors were found on item " + j.ToString());
                            Console.WriteLine("Error code is: " + err.statusCode.ToString());
                            Console.WriteLine("Error message: " + err.message);
                        }
                    }
                    Console.Write("\nHit return to continue...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to execute query succesfully, error message was: \n"
                    + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private string GetDateTimeSerialized(System.DateTime dateValue)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(dateValue.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ser.Serialize(ms, dateValue);
            byte[] b = ms.ToArray();
            string dt = System.Text.ASCIIEncoding.ASCII.GetString(b);
            System.Xml.XmlDocument d = new System.Xml.XmlDocument();
            d.LoadXml(dt);
            return d.InnerText;
        }

        private void createTaskSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                //create an array to create 4 items in one call
                partner.sObject[] taskarray = new partner.sObject[3];
                for (int x = 0; x < taskarray.Length; x++)
                {
                    //Declare a new task object to hold our values
                    partner.sObject task = new partner.sObject();
                    int index = 0;
                    System.Xml.XmlElement[] tsk = new System.Xml.XmlElement[5];
                    if (contacts != null) tsk = new System.Xml.XmlElement[tsk.Length + 1];
                    if (accounts != null) tsk = new System.Xml.XmlElement[tsk.Length + 1];
                    //make sure we get some errors on records 3 and 4
                    if (x > 1)
                    {
                        tsk = new System.Xml.XmlElement[tsk.Length + 1];
                        tsk[index++] = GetNewXmlElement("OwnerId", "DSF:LJKSDFLKJ");
                    }

                    //Set the appropriate values on the task
                    tsk[index++] = GetNewXmlElement("ActivityDate", GetDateTimeSerialized(DateTime.Now));
                    tsk[index++] = GetNewXmlElement("Description", "Get in touch with this person");
                    tsk[index++] = GetNewXmlElement("Priority", "Normal");
                    tsk[index++] = GetNewXmlElement("Status", "Not Started");
                    tsk[index++] = GetNewXmlElement("Subject", "Setup Call");
                    //The two lines below illustrate associating an object with another object.  If
                    //we have created an account and/or a contact prior to creating the task, we will
                    //just grab the first account and/or contact id and place it in the appropriate
                    //reference field.  WhoId can be a reference to a contact or a lead or a user.  
                    //WhatId can be a reference to an account, campaign, case, contract or opportunity
                    if (contacts != null) tsk[index++] = GetNewXmlElement("WhoId", contacts[0]);
                    if (accounts != null) tsk[index++] = GetNewXmlElement("WhatId", accounts[0]);
                    task.type = "Task";
                    taskarray[x] = task;
                }

                //call the create method passing the array of tasks as sobjects
                partner.SaveResult[] sr = binding.create(taskarray);

                for (int j = 0; j < sr.Length; j++)
                {
                    if (sr[j].success)
                    {
                        Console.WriteLine("A task was create with an id of: "
                            + sr[j].id);
                        if (accounts != null)
                            Console.WriteLine(" - the task was associated with the account you created with an id of "
                                + accounts[0]
                                + ".");


                        if (tasks == null)
                        {
                            tasks = new string[] { sr[j].id };
                        }
                        else
                        {
                            string[] tempTasks = null;
                            tempTasks = new string[tasks.Length + 1];
                            for (int i = 0; i < tasks.Length; i++)
                                tempTasks[i] = tasks[i];
                            tempTasks[tasks.Length] = sr[j].id;
                            tasks = tempTasks;
                        }

                    }
                    else
                    {
                        //there were errors during the create call, go through the errors
                        //array and write them to the screen
                        for (int i = 0; i < sr[j].errors.Length; i++)
                        {
                            //get the next error
                            partner.Error err = sr[j].errors[i];
                            Console.WriteLine("Errors were found on item " + j.ToString());
                            Console.WriteLine("Error code is: " + err.statusCode.ToString());
                            Console.WriteLine("Error message: " + err.message);
                        }
                    }
                    Console.WriteLine("\nCreate task successful.");
                }
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to succesfully create a task, error message was: \n"
                    + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private void querySample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            partner.QueryResult qr = null;
            binding.QueryOptionsValue = new partner.QueryOptions();
            binding.QueryOptionsValue.batchSize = 3;
            binding.QueryOptionsValue.batchSizeSpecified = true;

            try
            {
                qr = binding.query(
                    "select id, Website, Name from Account where Name = 'Golden Straw'");
                if (qr.size > 0)
                {
                    partner.sObject account = qr.records[0];
                    Console.WriteLine("Found " + qr.size.ToString() + " accounts using Name = 'Golden Straw', ID = " + account.Id + ", website = " + getFieldValue("Website", account.Any));
                }
                else
                {
                    Console.WriteLine("No records were found.  Try running the create account sample.");
                    Console.ReadLine();
                }

                qr = binding.query("select FirstName, LastName from Contact");

                bool bContinue = true;
                int loopCounter = 0;
                while (bContinue)
                {
                    Console.WriteLine("\nResults Set " + Convert.ToString(loopCounter++) + " - ");
                    //process the query results
                    for (int i = 0; i < qr.records.Length; i++)
                    {
                        partner.sObject con = qr.records[i];
                        string fName = getFieldValue("FirstName", con.Any);
                        string lName = getFieldValue("LastName", con.Any);
                        if (fName == null)
                            Console.WriteLine("Contact " + (i + 1) + ": " + lName);
                        else
                            Console.WriteLine("Contact " + (i + 1) + ": " + fName + " " + lName);
                    }
                    //handle the loop + 1 problem by checking to see if the most recent queryResult
                    if (qr.done)
                        bContinue = false;
                    else
                        qr = binding.queryMore(qr.queryLocator);
                }
                Console.WriteLine("\nQuery succesfully executed.");
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to execute query succesfully, error message was: \n"
                           + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }

        }

        private void retrieveSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            if (accounts == null)
            {
                Console.WriteLine("To run the retrieve sample, create one or more\naccounts using the Create Accounts Sample first.\n\nHit return to continue...");
            }
            else
            {
                try
                {
                    partner.sObject[] sobjects =
                        binding.retrieve(
                        "Id, AccountNumber, Name, Website",
                        "Account",
                        accounts);
                    if (sobjects != null)
                    {
                        Console.WriteLine("Returning data for " + sobjects.Length + " accounts: ");
                        for (int i = 0; i < sobjects.Length; i++)
                        {
                            partner.sObject acct = sobjects[i];
                            Console.WriteLine("Account Id: "
                                       + acct.Id);
                            Console.WriteLine("    AccountNumber = "
                                       + getFieldValue("AccountNumber", acct.Any));
                            Console.WriteLine("    Name          = "
                                       + getFieldValue("Name", acct.Any));
                            Console.WriteLine("    Website       = "
                                       + getFieldValue("Website", acct.Any) + "\n");
                        }
                        Console.Write("\nRetrive executed successfully, hit return to continue....");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nFailed to succesfully retrieve records, error message was: \n"
                               + ex.Message);
                    Console.Write("\nHit return to continue...");
                    Console.ReadLine();
                }
            }

        }

        private void deleteSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            //check to see if we know anything that was created
            if (tasks == null && contacts == null && accounts == null)
            {
                Console.WriteLine("\nDelete operation not completed.  You will need to create a task, account or contact during this session to run the delete sample.");
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
                return;
            }
            try
            {
                if (tasks != null)
                {
                    binding.delete(tasks);
                    Console.WriteLine("\nSuccessfully deleted " + tasks.Length + " tasks.");
                    tasks = null;
                }
                else
                {
                    Console.WriteLine("\nDeleted 0 tasks.");
                }
                if (contacts != null)
                {
                    binding.delete(contacts);
                    Console.WriteLine("\nSuccessfully deleted " + contacts.Length + " contacts.");
                    contacts = null;
                }
                else
                {
                    Console.WriteLine("\nDeleted 0 contacts.");
                }
                if (accounts != null)
                {
                    binding.delete(accounts);
                    Console.WriteLine("\nSuccessfully deleted " + accounts.Length + " accounts.");
                    accounts = null;
                }
                else
                {
                    Console.WriteLine("\nDeleted 0 accounts.");
                }
                Console.WriteLine("\nDelete sample completed successfully.");
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to succesfully create a task, error message was: \n"
                           + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private void describeGlobalSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                partner.DescribeGlobalResult describeGlobalResult = binding.describeGlobal();
                if (describeGlobalResult != null)
                {
                    String[] types = describeGlobalResult.types;
                    if (types != null)
                    {
                        for (int i = 0; i < types.Length; i++)
                        {
                            Console.WriteLine(types[i]);
                        }
                        Console.WriteLine("\nDescribe global was successful.\n\nHit the enter key to conutinue....");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to return types, error message was: \n"
                           + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private void getDeletedSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                //Calendar deletedDate 

                partner.GetDeletedResult gdr = binding.getDeleted("Account", serverTime.Subtract(new System.TimeSpan(0, 0, 5, 0, 0)), serverTime);

                if (gdr.deletedRecords != null)
                {
                    for (int i = 0; i < gdr.deletedRecords.Length; i++)
                    {
                        Console.WriteLine(gdr.deletedRecords[i].id + " was deleted on " + gdr.deletedRecords[i].deletedDate.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No deleted accounts since " + serverTime.Subtract(new System.TimeSpan(0, 0, 5, 0, 0)));
                }
                serverTime = binding.getServerTimestamp().timestamp;
                //Console.WriteLine("\nHit return to continue...");
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "\nFailed to execute query succesfully, error message was: \n"
                    + ex.Message);
                //Console.WriteLine("\nHit return to continue...");
                //Console.ReadLine();
            }
        }

        private void getUpdatedSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            try
            {

                partner.GetUpdatedResult gur = binding.getUpdated("Account", serverTime.Subtract(new System.TimeSpan(0, 0, 5, 0, 0)), serverTime);
                if (gur.ids != null)
                {
                    for (int i = 0; i < gur.ids.Length; i++)
                    {
                        Console.WriteLine(gur.ids[i] + " was updated or created.");
                    }
                }
                else
                {
                    Console.WriteLine("No updates to accounts since " + serverTime.Subtract(new System.TimeSpan(0, 0, 5, 0, 0)));
                }
                serverTime = binding.getServerTimestamp().timestamp;
                //Console.WriteLine("\nHit return to continue...");
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                           "\nFailed to execute query succesfully, error message was: \n"
                           + ex.Message);
                //Console.WriteLine("\nHit return to continue...");
                //Console.ReadLine();
            }
        }

        private string getFieldValue(string fieldName, System.Xml.XmlElement[] fields)
        {
            string returnValue = "";
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].LocalName.ToLower().Equals(fieldName.ToLower()))
                    {
                        returnValue = fields[i].InnerText;
                    }
                }
            }
            return returnValue;
        }

        private void searchSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            partner.SearchResult sr = null;
            try
            {
                sr = binding.search("find {4159017000} in phone fields returning " +
                                    "contact(id, phone, firstname, lastname), " +
                                    "lead(id, phone, firstname, lastname), " +
                                    "account(id, phone, name)");
                partner.SearchRecord[] records = sr.searchRecords;
                System.Collections.ArrayList contacts = new System.Collections.ArrayList();
                System.Collections.ArrayList leads = new System.Collections.ArrayList();
                System.Collections.ArrayList accounts = new System.Collections.ArrayList();

                if (records != null)
                {
                    for (int i = 0; i < records.Length; i++)
                    {
                        partner.SearchRecord record = records[i];
                        if (record.record.type.ToLower().Equals("contact"))
                        {
                            contacts.Add(record.record);
                        }
                        else if (record.record.type.ToLower().Equals("lead"))
                        {
                            leads.Add(record.record);
                        }
                        else if (record.record.type.ToLower().Equals("account"))
                        {
                            accounts.Add(record.record);
                        }
                        System.Diagnostics.Trace.WriteLine("out");
                    }
                    if (contacts.Count > 0)
                    {
                        Console.WriteLine("Found " + contacts.Count + " contacts:");
                        for (int i = 0; i < contacts.Count; i++)
                        {
                            partner.sObject c = (partner.sObject)contacts[i];
                            Console.WriteLine(getFieldValue("FirstName", c.Any) + " " + getFieldValue("LastName", c.Any) + " - " + getFieldValue("Phone", c.Any));
                        }
                    }
                    if (leads.Count > 0)
                    {
                        Console.WriteLine("Found " + leads.Count + " leads:");
                        for (int i = 0; i < leads.Count; i++)
                        {
                            partner.sObject l = (partner.sObject)leads[i];
                            Console.WriteLine(getFieldValue("FirstName", l.Any) + " " + getFieldValue("LastName", l.Any) + " - " + getFieldValue("Phone", l.Any));
                        }
                    }
                    if (accounts.Count > 0)
                    {
                        Console.WriteLine("Found " + accounts.Count + " accounts:");
                        for (int i = 0; i < accounts.Count; i++)
                        {
                            partner.sObject a = (partner.sObject)accounts[i];
                            Console.WriteLine(getFieldValue("Name", a.Any) + " - " + getFieldValue("Phone", a.Any));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No records were found for the search.");
                }

                Console.WriteLine("\nSearch succesfully executed.");
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nFailed to execute search succesfully, error message was: \n"
                           + e.Message);
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }

        }

        private void showSearchResults(partner.SearchResult sr)
        {
            partner.SearchRecord[] records = sr.searchRecords;
            System.Collections.ArrayList contacts = new System.Collections.ArrayList();
            System.Collections.ArrayList leads = new System.Collections.ArrayList();
            System.Collections.ArrayList accounts = new System.Collections.ArrayList();

            if (records != null)
            {
                for (int i = 0; i < records.Length; i++)
                {
                    partner.SearchRecord record = records[i];
                    if (record.record.type.ToLower().Equals("contact"))
                    {
                        contacts.Add(record.record);
                    }
                    else if (record.record.type.ToLower().Equals("lead"))
                    {
                        leads.Add(record.record);
                    }
                    else if (record.record.type.ToLower().Equals("account"))
                    {
                        accounts.Add(record.record);
                    }
                    System.Diagnostics.Trace.WriteLine("out");
                }
                if (accounts.Count > 0)
                {
                    Console.WriteLine("Found " + accounts.Count + " accounts:");
                    for (int i = 0; i < accounts.Count; i++)
                    {
                        partner.sObject a = (partner.sObject)accounts[i];
                        Console.WriteLine(getFieldValue("Name", a.Any) + " - " + getFieldValue("Phone", a.Any) + " state: " + getFieldValue("BillingState", a.Any));
                    }
                }
            }
            else
            {
                Console.WriteLine("No records were found for the search.");
            }
        }

        private void searchFilterSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            partner.SearchResult sr = null;
            try
            {
                Console.WriteLine("\nRunning unfiltered search...\n");
                sr = binding.search("find {Un*} in Name fields returning " +
                            "account(id, phone, name, BillingState )");
                showSearchResults(sr);

                Console.WriteLine("\nUnfiltered search succesfully executed.");
                Console.Write("Hit return to filter search using BillingState = AZ or NY...");
                Console.ReadLine();
                Console.WriteLine("\nRunning filtered search...\n");
                sr = binding.search("find {Un*} in Name fields returning " +
                            "account(id, phone, name, BillingState where billingstate In ('AZ', 'NY'))");
                showSearchResults(sr);

                Console.WriteLine("\nFiltered search succesfully executed.");
                Console.Write("Hit return to run filtered search...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nFailed to execute search succesfully, error message was: \n"
                           + e.Message);
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }

        }

        private void describeLayoutSample()
        {
            if (!loggedIn)
            {
                if (!login())
                {
                    return;
                }
            }

            try
            {
                Console.Write("Enter the name of an object to describe the layout of: ");
                string objectToDescribe = Console.ReadLine();
                partner.DescribeLayoutResult dlr = binding.describeLayout(objectToDescribe, null);
                Console.WriteLine("There are " + dlr.layouts.Length + " layouts for the " + objectToDescribe + " object.");
                for (int i = 0; i < dlr.layouts.Length; i++)
                {
                    partner.DescribeLayout layout = dlr.layouts[i];
                    Console.WriteLine("    There are " + layout.detailLayoutSections.Length + " detail layout sections");
                    for (int j = 0; j < layout.detailLayoutSections.Length; j++)
                    {
                        partner.DescribeLayoutSection dls = layout.detailLayoutSections[j];
                        Console.WriteLine(j + "        This section has a heading of " + dls.heading);
                    }
                    if (layout.editLayoutSections != null)
                    {
                        Console.WriteLine("    There are " + layout.editLayoutSections.Length + " edit layout sections");
                        for (int j = 0; j < layout.editLayoutSections.Length; j++)
                        {
                            partner.DescribeLayoutSection els = layout.editLayoutSections[j];
                            Console.WriteLine(j + "        This section has a heading of " + els.heading);
                            Console.WriteLine("This section has " + els.layoutRows.Length + " layout rows.");
                            for (int k = 0; k < els.layoutRows.Length; k++)
                            {
                                partner.DescribeLayoutRow lr = els.layoutRows[k];
                                Console.WriteLine("            This row has " + lr.numItems + " items.");
                                for (int h = 0; h < lr.layoutItems.Length; h++)
                                {
                                    partner.DescribeLayoutItem li = lr.layoutItems[h];
                                    if (li.layoutComponents != null)
                                        Console.WriteLine("                " + h + " " + li.layoutComponents[0].value);
                                }
                            }
                        }
                    }
                }
                if (dlr.recordTypeMappings != null)
                    Console.WriteLine("There are " + dlr.recordTypeMappings.Length + " record type mappings for the " + objectToDescribe + " object");
                else
                    Console.WriteLine("There are no record type mappings for the " + objectToDescribe + " object.");

            }
            catch (Exception e)
            {
                Console.WriteLine("An exceptions was caught: " + e.Message);
            }
        }
        private void describeSObjectsSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                partner.DescribeSObjectResult[] describeSObjectResults = binding.describeSObjects(new string[] { "account", "contact", "lead" });
                for (int x = 0; x < describeSObjectResults.Length; x++)
                {
                    partner.DescribeSObjectResult describeSObjectResult = describeSObjectResults[x];
                    Console.WriteLine("\n\n" + describeSObjectResult.name);
                    // Retrieve fields from the results
                    partner.Field[] fields = describeSObjectResult.fields;
                    // Get the name of the object
                    String objectName = describeSObjectResult.name;
                    // Get some flags
                    bool isActivateable = describeSObjectResult.activateable;
                    // Many other values are accessible
                    if (fields != null)
                    {
                        // Iterate through the fields to get properties for each field
                        for (int i = 0; i < fields.Length; i++)
                        {
                            partner.Field field = fields[i];
                            int byteLength = field.byteLength;
                            int digits = field.digits;
                            string label = field.label;
                            int length = field.length;
                            string name = field.name;
                            partner.PicklistEntry[] picklistValues = field.picklistValues;
                            int precision = field.precision;
                            string[] referenceTos = field.referenceTo;
                            int scale = field.scale;
                            partner.fieldType fieldType = field.type;
                            bool fieldIsCreateable = field.createable;
                            // Determine whether there are picklist values
                            if (picklistValues != null && picklistValues[0] != null)
                            {
                                Console.WriteLine("Picklist values = ");
                                for (int j = 0; j < picklistValues.Length; j++)
                                {
                                    if (picklistValues[j].label != null)
                                    {
                                        Console.WriteLine(" Item: " +
                                            picklistValues[j].label);
                                    }
                                }
                            }
                            // Determine whether this field refers to another object
                            if (referenceTos != null && referenceTos[0] != null)
                            {
                                Console.WriteLine("Field references the following objects:");
                                for (int j = 0; j < referenceTos.Length; j++)
                                {
                                    Console.WriteLine(" " + referenceTos[j]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on DescribeSObjects: " + ex.Message);
            }
        }
        private void describeTabsSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }

            try
            {
                partner.DescribeTabSetResult[] dtsrs = binding.describeTabs();

                Console.WriteLine("There are " + dtsrs.Length.ToString() + " tabsets defined.");
                for (int i = 0; i < dtsrs.Length; i++)
                {
                    Console.WriteLine("Tabset " + (i + 1).ToString() + ":");
                    partner.DescribeTabSetResult dtsr = dtsrs[i];
                    String tabSetLabel = dtsr.label;
                    String logoUrl = dtsr.logoUrl;
                    bool isSelected = dtsr.selected;
                    partner.DescribeTab[] tabs = dtsr.tabs;
                    Console.WriteLine("Label is " + tabSetLabel + " logo url is " + logoUrl + ", there are " + tabs.Length.ToString() + " tabs defined in this set.");
                    for (int j = 0; j < tabs.Length; j++)
                    {
                        partner.DescribeTab tab = tabs[j];
                        String tabLabel = tab.label;
                        String objectName = tab.sobjectName;
                        String tabUrl = tab.url;
                        Console.WriteLine("\tTab " + (j + 1).ToString() + ": \n\t\tLabel = " + tabLabel +
                            "\n\t\tObject details on tab: " + objectName + "\n\t\t" +
                            "Url to tab: " + tabUrl);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on DescribeTab: " + ex.Message);
            }
        }

        private void createCustomObjectSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            try
            {
                /*
                MetadataService mBinding = new apexMetadata.MetadataService();
                mBinding.SessionHeaderValue = new basicSample_cs_p.apexMetadata.SessionHeader();
                mBinding.SessionHeaderValue.sessionId = binding.SessionHeaderValue.sessionId;
                mBinding.Url = binding.Url.Replace(@"/u/", @"/m/");

                CustomObject cObj = new CustomObject();
                cObj.deploymentStatus = DeploymentStatus.Deployed;
                cObj.description = "This object was created using the metadata API sample.";
                Console.Write("Do you want to enable activities on this new object? (y/n)");
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    cObj.enableActivities = true;
                }
                Console.Write("Do you want to enable division support on this new object? (y/n)");
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    cObj.enableDivisions = true;
                }
                Console.Write("Do you want to enable history on this new object? (y/n)");
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    cObj.enableHistory = true;
                }
                Console.Write("Do you want to enable reporting on this new object? (y/n)");
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    cObj.enableReports = true;
                }
                Console.Write("Enter the name of your custom object: ");
                String fullName = Console.ReadLine();
                if (fullName.Length == 0)
                {
                    Console.Write("No name entered, quitings custom object creation...");
                    Console.ReadLine();
                    return;
                }
                cObj.fullName = fullName + "__c";
                Console.Write("Enter the label for your custom object: ");
                String objectLabel = Console.ReadLine();
                if (objectLabel.Length == 0)
                {
                    Console.Write("No label entered, quiting custom object creation...");
                    Console.ReadLine();
                    return;
                }
                cObj.label = objectLabel;
                CustomField nameField = new CustomField();
                Console.Write("Enter the name field name for the your object: ");
                nameField.fullName = Console.ReadLine();
                if (nameField.fullName.Length == 0)
                {
                    return;
                }
                Console.Write("What type of field is the name field? (text/autonumber) ");
                String nameFieldType = Console.ReadLine();
                if (nameFieldType.Length == 0)
                {
                    return;
                }
                else
                {
                    if (nameFieldType.ToLower().Equals("text"))
                    {
                        nameField.type = FieldType.Text;
                        nameField.length = 255;
                    }
                    else if (nameFieldType.ToLower().Equals("autonumber"))
                    {
                        nameField.startingNumber = 1;
                        nameField.displayFormat = "SAMPLE {000000}";
                    }
                    else
                    {
                        return;
                    }
                    cObj.nameField = nameField;
                }
                cObj.nameField.description = "This is the name field from the API Samples.";
                cObj.nameField.label = cObj.fullName + " name";
                cObj.pluralLabel = cObj.label + "s";
                cObj.sharingModel = SharingModel.ReadWrite;
                cObj.gender = null;
                cObj.startsWith = null;
                AsyncResult asyncResult = mBinding.create(new Metadata[] { cObj })[0];
                while (asyncResult.state == AsyncRequestState.InProgress)
                {
                    Thread.Sleep(asyncResult.secondsToWait * 1000);
                    Console.WriteLine("Checking status...\n\tState:" + asyncResult.state + "\n\tStatus: " + asyncResult.statusCode + "\n\tSeconds to wait: " + asyncResult.secondsToWait);
                    asyncResult = mBinding.checkStatus(new String[] { asyncResult.id })[0];
                }
                if (asyncResult.state == AsyncRequestState.Completed)
                {
                    Console.Write("Custom object has been created. \n\nUse the describe global sample to see your object listed.");
                    Console.ReadLine();
                }
                Console.Write("To delete the newly created custom object enter 'delete' now... ");
                if (Console.ReadLine().ToLower().Equals("delete"))
                {
                    CustomObject deleteObj = new CustomObject();
                    deleteObj.fullName = cObj.fullName;
                    asyncResult = mBinding.delete(new Metadata[] { deleteObj })[0];
                    while (asyncResult.state == AsyncRequestState.InProgress)
                    {
                        Thread.Sleep(asyncResult.secondsToWait * 1000);
                        Console.WriteLine("Checking status...\n\tState:" + asyncResult.state + "\n\tStatus: " + asyncResult.statusCode + "\n\tSeconds to wait: " + asyncResult.secondsToWait);
                        asyncResult = mBinding.checkStatus(new String[] { asyncResult.id })[0];
                    }
                    if (asyncResult.state == AsyncRequestState.Completed)
                    {
                        Console.Write("Custom object has been deleted. \n\nUse the describe global sample to see your object is not listed.");
                        Console.ReadLine();
                    }
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to create object: \n"
                           + ex.Message);
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private void createCustomFieldSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            try
            {
                Console.WriteLine("\nThis sample will create a new Text field on the Lead object.");
                Console.ReadLine();
                Console.Write("Enter the name that you would like for the field: ");
                String fieldName = Console.ReadLine();
                if (fieldName.Length == 0)
                {
                    Console.Write("No name was entered for the field, so we are exiting this sample.");
                    Console.ReadLine();
                    return;
                }
               /* CustomField cf = new CustomField();
                cf.description = "Simple text field from API";
                cf.fullName = "Lead." + fieldName + "__c";
                Console.Write("Enter a label for the field: ");
                String fieldLabel = Console.ReadLine();
                if (fieldLabel.Length == 0)
                {
                    fieldLabel = "Sample Field";
                }
                cf.label = fieldLabel;
                cf.length = 50;
                cf.type = FieldType.Text;

                MetadataService mBinding = new MetadataService();
                mBinding.SessionHeaderValue = new basicSample_cs_p.apexMetadata.SessionHeader();
                mBinding.SessionHeaderValue.sessionId = binding.SessionHeaderValue.sessionId;
                mBinding.Url = binding.Url.Replace(@"/u/", @"/m/");

                AsyncResult asyncResult = mBinding.create(new Metadata[] { cf })[0];

                while (asyncResult.state == AsyncRequestState.InProgress)
                {
                    Thread.Sleep(asyncResult.secondsToWait * 1000);
                    Console.WriteLine("Checking status...\n\tState:" + asyncResult.state + "\n\tStatus: " + asyncResult.statusCode + "\n\tSeconds to wait: " + asyncResult.secondsToWait);
                    asyncResult = mBinding.checkStatus(new String[] { asyncResult.id })[0];
                }
                if (asyncResult.state == AsyncRequestState.Completed)
                {
                    Console.Write("Custom object has been created. \n\nUse the describe sobject sample to see your object is not listed.");
                    Console.ReadLine();
                }*/

            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to create object: \n"
                           + ex.Message);
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private void describeSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            Console.Write("\nType the name of the object to describe (try Account): ");
            string objectToDescribe = Console.ReadLine();
            try
            {
                partner.DescribeSObjectResult describeSObjectResult = binding.describeSObject(objectToDescribe);
                if (describeSObjectResult != null)
                {
                    partner.Field[] fields = describeSObjectResult.fields;
                    String objectName = describeSObjectResult.name;
                    bool isActivateable = describeSObjectResult.activateable;
                    bool isCreateable = describeSObjectResult.createable;
                    bool isCustom = describeSObjectResult.custom;
                    bool isDeleteable = describeSObjectResult.deletable;
                    bool isQueryable = describeSObjectResult.queryable;
                    bool isReplicateable = describeSObjectResult.replicateable;
                    bool isRetrieveable = describeSObjectResult.retrieveable;
                    bool isSearchable = describeSObjectResult.searchable;
                    bool isUndeleteable = describeSObjectResult.undeletable;
                    bool isUpdateable = describeSObjectResult.updateable;
                    
                    Console.WriteLine("Metadata for " + objectToDescribe + " object:\n");
                    Console.WriteLine("Object name                       = " + objectName);
                    Console.WriteLine("Number of fields                  = " + fields.Length);
                    Console.WriteLine("Object can be activated           = " + isActivateable);
                    Console.WriteLine("Can create rows of data           = " + isCreateable);
                    Console.WriteLine("Object is custom object           = " + isCustom);
                    Console.WriteLine("Can delete rows of data           = " + isDeleteable);
                    Console.WriteLine("Can query for rows of data        = " + isQueryable);
                    Console.WriteLine("Object can be used in replication = " + isReplicateable);
                    Console.WriteLine("Can use retrieve method on object = " + isRetrieveable);
                    Console.WriteLine("Can use search method on object   = " + isSearchable);
                    Console.WriteLine("Can un-delete rows of data        = " + isUndeleteable);
                    Console.WriteLine("Can update rows of data           = " + isUpdateable);
                    Console.WriteLine("\nField metadata for " + objectToDescribe + " object:\n");

                    if (fields != null)
                    {
                        for (int i = 0; i < fields.Length; i++)
                        {
                            partner.Field field = fields[i];
                            int byteLength = field.byteLength;
                            int digits = field.digits;
                            String label = field.label;
                            int length = field.length;
                            String name = field.name;
                            partner.PicklistEntry[] picklistValues =
                                field.picklistValues;
                            int precision = field.precision;
                            String[] referenceTos = field.referenceTo;
                            int scale = field.scale;
                            partner.fieldType fieldType = field.type;
                            bool fieldIsCreateable = field.createable;
                            bool fieldIsCustom = field.custom;
                            bool fieldIsFilterable = field.filterable;
                            bool fieldIsNillable = field.nillable;
                            bool fieldIsRestrictedPicklist = field.restrictedPicklist;
                            bool fieldIsUpdateable = field.updateable;
                            Console.WriteLine(
                                       "*************  New Field  ***************");
                            Console.WriteLine("Name            = " + name);
                            Console.WriteLine("Label           = " + label);
                            Console.WriteLine("Length          = " + length);
                            Console.WriteLine("Bytelength      = " + byteLength);
                            Console.WriteLine("Digits          = " + digits);
                            Console.WriteLine("Precision       = " + precision);
                            Console.WriteLine("Scale           = " + scale);
                            Console.WriteLine("Field type      = " + fieldType);
                            if (picklistValues != null && picklistValues[0] != null)
                            {
                                Console.WriteLine("Picklist values = ");
                                for (int j = 0; j < picklistValues.Length; j++)
                                {
                                    if (picklistValues[j].label != null)
                                        Console.WriteLine("    Item:  "
                                                   + picklistValues[j].label);
                                    else
                                        Console.WriteLine("    Item:  "
                                                   + picklistValues[j].value);
                                    Console.WriteLine("        value           = "
                                               + picklistValues[j].value);
                                    Console.WriteLine("        is default      = "
                                               + picklistValues[j].defaultValue);
                                }
                            }
                            if (referenceTos != null && referenceTos[0] != null)
                            {
                                Console.WriteLine("Field references the following objects:");
                                for (int j = 0; j < referenceTos.Length; j++)
                                    Console.WriteLine("    " + referenceTos[j]);
                            }
                            Console.WriteLine("\n");
                        }
                        Console.Write("\nDescribe "
                            + objectToDescribe
                            + " was successful.\n\nHit the enter key to conutinue....");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to "
                           + objectToDescribe
                           + " description, error message was: \n"
                           + ex.Message);
                Console.WriteLine("\nHit return to continue...");
                Console.ReadLine();
            }
        }

        private void setPasswordSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            try
            {
                partner.QueryResult qr = binding.query(
                    "select UserName, LastName, FirstName, Id from User");
                if (qr != null)
                {
                    partner.sObject[] users = qr.records;
                    if (users != null)
                    {
                        Console.WriteLine("\nUser List: ");
                        for (int i = 0; i < users.Length; i++)
                        {
                            int printInd = i + 1;
                            partner.sObject user = users[i];
                            Console.WriteLine(printInd + ". " + getFieldValue("Username", user.Any) + " - " + getFieldValue("FirstName", user.Any) + " " + getFieldValue("LastName", user.Any));
                        }
                    }
                    Console.Write("\nEnter user to set password for: ");
                    String idToReset = Console.ReadLine();
                    if (idToReset != null)
                    {
                        int userIndex = Convert.ToInt16(idToReset) - 1;
                        Console.Write("Enter the new password: ");
                        String newPassword = Console.ReadLine();
                        if (newPassword != null)
                        {
                            Console.Write("Please verify that you want to reset the password for \n" + getFieldValue("FirstName", users[userIndex].Any) + " " + getFieldValue("LastName", users[userIndex].Any) + "\n to " + newPassword + " by entering OK.");
                            String verify = Console.ReadLine();
                            if (verify.Equals("OK"))
                            {
                                partner.SetPasswordResult setPasswordResult =
                                    binding.setPassword(users[userIndex].Id, newPassword);
                                if (setPasswordResult != null)
                                {
                                    Console.WriteLine("\nThe password for user " + getFieldValue("FirstName", users[userIndex].Any) + " " + getFieldValue("LastName", users[userIndex].Any) + " has been reset to " + newPassword);
                                    Console.Write("\nHit enter to continue...");
                                    Console.ReadLine();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to succesfully reset password, error message was: \n"
                    + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
            Console.Write("No password was set....\nHit return to continue...");
            Console.ReadLine();
        }

        private void resetPasswordSample()
        {
            //Verify that we are already authenticated, if not
            //call the login function to do so
            if (!loggedIn)
            {
                if (!login())
                    return;
            }
            try
            {
                partner.QueryResult qr = binding.query(
                    "select UserName, LastName, FirstName, Id from User");
                if (qr != null)
                {
                    partner.sObject[] users = qr.records;
                    if (users != null)
                    {
                        Console.WriteLine("\nUser List: ");
                        for (int i = 0; i < users.Length; i++)
                        {
                            int printInd = i + 1;
                            partner.sObject user = users[i];
                            Console.WriteLine(
                                       printInd
                                       + ". "
                                       + getFieldValue("Username", user.Any)
                                       + " - "
                                       + getFieldValue("FirstName", user.Any)
                                       + " "
                                       + getFieldValue("LastName", user.Any));
                        }
                    }
                    Console.Write("\nEnter user to reset: ");
                    String idToReset = Console.ReadLine();
                    if (idToReset != null)
                    {
                        int userIndex = Convert.ToInt16(idToReset) - 1;
                        Console.Write("Please verify that you want to reset the password for \n"
                            + getFieldValue("FirstName", users[userIndex].Any)
                            + " "
                            + getFieldValue("LastName", users[userIndex].Any)
                            + "\nby entering OK.");
                        String verify = Console.ReadLine();
                        if (verify.Equals("OK"))
                        {
                            partner.ResetPasswordResult resetPasswordResult =
                                binding.resetPassword(users[userIndex].Id);
                            if (resetPasswordResult != null)
                            {
                                Console.WriteLine("\nThe password for user "
                                           + getFieldValue("FirstName", users[userIndex].Any)
                                           + " "
                                           + getFieldValue("LastName", users[userIndex].Any)
                                           + " has been reset to "
                                           + resetPasswordResult.password);
                                Console.Write("\nHit enter to continue...");
                                Console.ReadLine();
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFailed to succesfully reset password, error message was: \n"
                           + ex.Message);
                Console.Write("\nHit return to continue...");
                Console.ReadLine();
            }
            Console.Write("No password was reset....\nHit return to continue...");
            Console.ReadLine();
        }




        /// <summary>
        /// Call SFDC endpoint and retrieve authentication token and API URL for SOAP callers
        /// </summary>
        private static void AuthenticateSfdcEnterpriseUser(string _un,string _pw,string _token)
        {
            //print message to console
            Console.WriteLine("Authenticating against the Enterprise API ...");

            //use default binding and address from app.config
            using (enterprise.SoapClient loginClient = new enterprise.SoapClient("Soap"))
            {
                //set account password and account token variables
                string sfdcPassword = _pw;
                string sfdcToken = _token;

                //set to Force.com user account that has API access enabled
                string sfdcUserName = _un;

                //create login password value
                string loginPassword = sfdcPassword + sfdcToken;

                
                //call Login operation from Enterprise WSDL
                enterprise.LoginResult result = 
                    loginClient.login(
                    null, //LoginScopeHeader
                    sfdcUserName,
                    loginPassword);

                //get response values
                sessionId = result.sessionId;
                serverUrl = result.serverUrl;

                //print response values
                Console.WriteLine(string.Format("The session ID is {0} and server URL is {1}", sessionId, serverUrl));
                Console.WriteLine("");
                Console.WriteLine("Press [Enter] to continue ...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Call SFDC endpoint and retrieve authentication token and API URL for SOAP callers
        /// </summary>
        private static void AuthenticateSfdcPartnerUser(string _un,string _pw,string _token)
        {
            //print message to console
            Console.WriteLine("Authenticating against the Partner API ...");

            //use default binding and address from app.config
            using (partner.SoapClient loginClient = new partner.SoapClient("Soap1"))
            {
                //set account password and account token variables
                string sfdcPassword = _pw;
                string sfdcToken = _token;

                //set to Force.com user account that has API access enabled
                string sfdcUserName = _un;

                //create login password value
                string loginPassword = sfdcPassword + sfdcToken;

                //call Login operation from Enterprise WSDL
                partner.LoginResult result =
                    loginClient.login(
                    null, //LoginScopeHeader
                    null, //calloptions
                    sfdcUserName,
                    loginPassword);

                //get response values
                sessionId = result.sessionId;
                serverUrl = result.serverUrl;

                //print response values
                Console.WriteLine(string.Format("The session ID is {0} and server URL is {1}", sessionId, serverUrl));
                Console.WriteLine("");
                Console.WriteLine("Press [Enter] to continue ...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Use Enteprise API to query and retrieve SFDC records
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="serverUrl"></param>
        private static void QueryEnterpriseRecord()
        {
            Console.WriteLine("Querying account records with the Enterprise API ...");

            //set query endpoint to value returned by login request
            EndpointAddress apiAddr = new EndpointAddress(serverUrl);

            //instantiate session header object and set session id
            enterprise.SessionHeader header = new enterprise.SessionHeader();
            header.sessionId = sessionId;

            //create service client to call API endpoint
            using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
            {
                //query standard or custom objects

                //create SOQL query statement
                string query = "SELECT Name, AccountNumber, BillingState FROM Account WHERE BillingState = 'CA'";
                
                enterprise.QueryResult result = queryClient.query(
                    header, //sessionheader
                    null, //queryoptions
                    null, //mruheader
                    null, //packageversion
                    query);

                //cast query results
                IEnumerable<enterprise.Account> accountList = result.records.Cast<enterprise.Account>();
                
                //show results
                foreach (var account in accountList)
                {
                    Console.WriteLine(string.Format("Account Name: {0}", account.Name));
                }

                Console.WriteLine("");
                Console.WriteLine("Query complete.");
                Console.ReadLine();

                //retrieve example

                //call retrieve operation to get one or more records of a given type and ID
                enterprise.sObject[] retrievedAccounts = queryClient.retrieve(
                    header, //sessionheader
                    null, //queryoptions
                    null, //mruheader
                    null, //packageversion
                    "Name, BillingState", //fieldlist
                    "Account", //objectype
                    new string[] { "001E000000N1H1O" } //record IDs
                    );

                foreach (enterprise.sObject so in retrievedAccounts)
                {
                    enterprise.Account acct = (enterprise.Account)so;
                    Console.WriteLine(string.Format("Account Name: {0}, Account State: {1}", acct.Name, acct.BillingState));
                }

                Console.WriteLine("");
                Console.WriteLine("Retrieve complete.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Use Enterprise API to create new SFDC records
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="serverUrl"></param>
        private static void CreateEnterpriseRecords()
        {
             Console.WriteLine("Creating an account record with the Enterprise API ...");

            //set query endpoint to value returned by login request
            EndpointAddress apiAddr = new EndpointAddress(serverUrl);

            //instantiate session header object and set session id
            enterprise.SessionHeader header = new enterprise.SessionHeader();
            header.sessionId = sessionId;

            //create service client to call API endpoint
            using (enterprise.SoapClient createClient = new enterprise.SoapClient("Soap", apiAddr))
            {
                enterprise.Account newAcct = new enterprise.Account();
                newAcct.Name = "DevForce02";
                newAcct.AccountNumber = "10043332";
                //all non-string fields must have their corresponding <name>Specified property set
                newAcct.AnnualRevenue = 4000000f;
                //newAcct.AnnualRevenueSpecified = true;

                enterprise.Opportunity o = new enterprise.Opportunity();
                o.Name = "Opp2";
                o.StageName = "Prospecting";
                o.CloseDate = DateTime.Parse("2013-03-22");
                o.CloseDateSpecified = true;

                enterprise.SaveResult[] results;

                createClient.create(
                    header, //sessionheader
                    null, //assignmentruleheader
                    null, //mruheader
                    null, //allowfieldtruncationheader
                    null, //disablefeedtrackingheader
                    null, //streamingenabledheader
                    null, //allornoneheader
                    null, //debuggingheader
                    null, //packageversionheader
                    null, //emailheader
                    new enterprise.sObject[] { o }, //objects to add
                    out results //results of the creation operation
                    );

                //only added one item, so looking at first index of results object
                if (results[0].success)
                {
                    Console.WriteLine("Account successfully created.");
                }
                else
                {
                    Console.WriteLine(results[0].errors[0].message);
                }

                Console.ReadLine();
            }
        }

        /// <summary>
        /// Use Partner API to query SFDC records
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="serverUrl"></param>
        private static void QueryPartnerRecords()
        {
            Console.WriteLine("Querying account records with the Partner API ...");

            //set query endpoint to value returned by login request
            EndpointAddress apiAddr = new EndpointAddress(serverUrl);

            partner.SessionHeader header = new partner.SessionHeader();
            header.sessionId = sessionId;

            //create service client to call API endpoint
            using (partner.SoapClient queryClient = new partner.SoapClient("Soap1", apiAddr))
            {
                //create SOQL query statement
                string query = "SELECT Name, AccountNumber, BillingState FROM Account";

                partner.QueryResult result = queryClient.query(
                    header, //sessionheader
                    null, //calloptions
                    null, //queryoptions
                    null, //mruheader
                    null, //packageversionheader
                    query //query string
                    );

                //loop through results
                foreach (partner.sObject account in result.records)
                {
                    string acctName = account.Any[0].InnerText;
                    string acctNum = account.Any.First(x => x.LocalName == "AccountNumber").InnerText;
                    Console.WriteLine(string.Format("Account Name: {0}, Account Number: {1}", acctName, acctNum));

                }

                Console.WriteLine("");
                Console.WriteLine("Query complete.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Use Partner API to create new SFDC records
        /// </summary>
        private static void CreatePartnerRecords()
        {
            Console.WriteLine("Creating an account record with the Partner API ...");

            //set query endpoint to value returned by login request
            EndpointAddress apiAddr = new EndpointAddress(serverUrl);

            partner.SessionHeader header = new partner.SessionHeader();
            header.sessionId = sessionId;

            //create service client to call API endpoint
            using (partner.SoapClient queryClient = new partner.SoapClient("Soap1", apiAddr))
            {
                partner.sObject account = new partner.sObject();
                account.type = "Account";

                //create XML containers for necessary XML document and elements
                XmlDocument rootDoc = new XmlDocument();
                XmlElement[] accountFields = new XmlElement[3];

                XElement[] test = new XElement[2];
                test[0] = new XElement("node1", "value");

                //add fields
                accountFields[0] = rootDoc.CreateElement("Name");
                accountFields[0].InnerText = "DevForce06";
                accountFields[1] = rootDoc.CreateElement("AccountNumber");
                accountFields[1].InnerText = "1004441239";
                accountFields[2] = rootDoc.CreateElement("AnnualRevenue");
                accountFields[2].InnerText = "4000000";

                //set object property to array
                account.Any = accountFields;

                partner.SaveResult[] results;
            
                queryClient.create(
                    header, //sessionheader
                    null,   //calloptions
                    null,   //assignmentruleheader
                    null,   //mruheader
                    null,   //allowfieldtruncationheader
                    null,   //disablefeedtrackingheader
                    null,   //streamingenabledheader
                    null,   //allornothingheader
                    null,   //debuggingheader
                    null,   //packageversionheader
                    null,   //emailheader
                    new partner.sObject[] { account }, //new accounts
                    out results //result of create operation
                    );


                //only added one item, so looking at first index of results object
                if (results[0].success)
                {
                    Console.WriteLine("Account successfully created.");
                }

                Console.ReadLine();
            }
        }
    } //end of class


}
