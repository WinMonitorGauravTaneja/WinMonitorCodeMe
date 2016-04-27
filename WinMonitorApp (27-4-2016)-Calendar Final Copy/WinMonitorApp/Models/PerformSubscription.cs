﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Microsoft.Exchange.WebServices.Data;

namespace WinMonitorApp.Models
{
    public class PerformSubscription
    {
        // To generate sequences from the self generated sequences in the database
        public string getseqDBSubscriptionsId()
        {
            WinMonitorEntityModelContext mDBContext = new WinMonitorEntityModelContext();
            string intnextseqDBSubscriptionsId = "Sub" + mDBContext.Database.SqlQuery<int>("select next value for seqDBSubscriptionsId").FirstOrDefault().ToString();
            return intnextseqDBSubscriptionsId;
        }

        //method to add subscriptions in the database
        public string mAddSubscription(string pstrName, string pstrEmail, string pstrId)
        {
            WinMonitorEntityModelContext mDBContext = new WinMonitorEntityModelContext();
            DBSubscription SubscriptionObj = new DBSubscription();
            SubscriptionObj.DBSubscriptionsId = getseqDBSubscriptionsId();
            SubscriptionObj.DBName = pstrName;
            SubscriptionObj.DBEmail = pstrEmail;
            SubscriptionObj.DBCompanyId = pstrId;

            int limitSubscriptionsPerCompany = mDBContext.Database.SqlQuery<Int32>("select count(*) from DBSubscriptions where DBCompanyId='"+ pstrId +"';").FirstOrDefault();

            try
            {
                if(limitSubscriptionsPerCompany <5)
                {
                mDBContext.DBSubscriptions.Add(SubscriptionObj);
                mDBContext.SaveChanges();
                return "Sucess, Subscriptions left for company:'"+ (4-limitSubscriptionsPerCompany) +"'";
                }
                else
                {
                    return "Unsucessful: Subscriptions limit: 5 for company reached";
                }
            }
            catch (DbUpdateException exUpdateDB)
            {
                Console.Write(exUpdateDB);
                return "DbUpdateException";
            }
            catch (DbEntityValidationException exEntityValidateDB)
            {
                Console.Write(exEntityValidateDB);
                return "DbEntityValidationException";
            }
            catch (NotSupportedException exNotSupportedDB)
            {
                Console.Write(exNotSupportedDB);
                return "NotSupportedException";
            }
            catch (ObjectDisposedException exObjectDisposedDB)
            {
                Console.Write(exObjectDisposedDB);
                return "ObjectDisposedException";
            }
            catch (InvalidOperationException exInvalidOperationDB)
            {
                Console.Write(exInvalidOperationDB);
                return "InvalidOperationException";
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return "Misllaneous Exception";
            }
        }


        //pseudocode for email transfer using subscriptions
        
        public void mSendMail(){
         ExchangeService objExchangeService = new ExchangeService(ExchangeVersion.Exchange2013);
			objExchangeService.Credentials = new WebCredentials("wse\\centraluser", "$abcd1234");
			objExchangeService.UseDefaultCredentials = false;
			objExchangeService.Url = new Uri("https://mail.winshuttle.in/EWS/Exchange.asmx");
			EmailMessage objMessage = new EmailMessage(objExchangeService);
			objMessage.ToRecipients.Add("harsimran.dhami@winshuttle.com");
			objMessage.CcRecipients.Add("harsimran.dhami@gmail.com");
			objMessage.Subject = "Sample Subject";
			objMessage.ReplyTo.Add(new EmailAddress("donotreply@winshuttle.com"));
			objMessage.Body = new MessageBody("<html><body><b>My Sample Content</b></body></html>");
			objMessage.Body.BodyType = BodyType.HTML;
			objMessage.Send();
    }


    }
}