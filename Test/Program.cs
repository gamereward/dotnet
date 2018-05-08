using GameReward;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    class Program
    {
        const String scoreType = "GAME1_SCORE_TYPE";
        static String username;
        static String userSelect;
        static void clearScreen()
        {
            Console.Clear();
        }

        static void pauseScreen()
        {
            Console.Write("Press enter to continue...");
            Console.Read();
        }
        static void printError(GrdResultBase error)
        {
            String input;
            Console.WriteLine("ERROR:" + error.error + ",MESSAGE:" + error.message);
            pauseScreen();
        }
        static String formatField(String st, int len)
        {
            String sfixed = "                                                                                                                       ";
            if (st.Length < len)
            {
                st = sfixed.Substring(sfixed.Length - len + st.Length) + st;
            }
            return st;
        }
        /*LEADER BOARD*/

        static void game_leaderboard(String title, String scoreType)
        {
            Console.WriteLine(title);
            Console.WriteLine("*********************************************************");
            GrdResult<GrdLeaderBoard[]> result = GrdManager.GetLeaderBoard(username, scoreType, 0, 20);
            if (result.error == 0)
            {
                Console.WriteLine("+-RANK---+----NAME------------------------------------+---SCORE--+");
                foreach (GrdLeaderBoard item in result.data)
                {
                    Console.WriteLine("|" + formatField(item.rank + "", 8) + "|" + formatField(item.username, 44) + "|" + formatField(item.score + "", 10) + "|");
                    Console.WriteLine("+--------+--------------------------------------------+----------+");
                }
                pauseScreen();
            }
            else
            {
                printError(result);
            }
        }
        /*ACCOUNT API TEST*/
        static void transfer()
        {
            clearScreen();
            String to;
            String svalue;
            decimal value = decimal.Zero;
            Console.WriteLine("TRANSFER MONEY");
            Console.WriteLine("-----------------------------------");
            Console.Write("TO ADDRESS:");
            to = Console.ReadLine(); ;
            if (to.Length == 0)
            {
                Console.WriteLine("INVALID ADDRESS!");
                pauseScreen();
                return;
            }
            do
            {
                Console.Write("AMOUNT:");
                svalue = Console.ReadLine();
                try
                {
                    value = decimal.Parse(svalue);
                }
                catch (Exception e)
                {
                }
                if (value <= 0)
                {
                    Console.WriteLine("The money need to be greater than 0.");
                }
                else
                {
                    break;
                }
            } while (true);
            Console.WriteLine("***************WARNING*************");
            Console.WriteLine("IT IS REAL MONEY!");
            Console.WriteLine("This action will transfer money from this account to " + to + "");
            Console.WriteLine("***********************************");
            Console.Write("Please confirm this action (YES to confirm,other to cancel):");
            svalue = Console.ReadLine();
            if (svalue.Equals("YES"))
            {

                GrdResultBase result = GrdManager.Transfer(username, to, value);
                if (result.error != 0)
                {
                    printError(result);
                    return;
                }
                else
                {
                    Console.WriteLine("TRANSFER SUCCESSFULLY!");
                    pauseScreen();
                    return;
                }
            }
        }
        static void chargeMoney()
        {
            Console.WriteLine("CHARGE MONEY");
            String svalue;
            decimal value = 0;
            Console.WriteLine("-----------------------------------");
            do
            {
                Console.Write("AMOUNT TO CHARGE:");
                svalue = Console.ReadLine();
                try
                {
                    value = decimal.Parse(svalue);
                }
                catch (Exception e)
                {
                }
                if (value <= 0)
                {
                    Console.WriteLine("The money need to be greater than 0.");
                }
                else
                {
                    break;
                }
            } while (true);
            GrdResultBase result = GrdManager.ChargeMoney(username, value);
            if (result.error != 0)
            {
                printError(result);
            }
            else
            {
                Console.WriteLine("CHARGE SUCCESSFULLY!");
                pauseScreen();
            }
        }

        static void payMoney()
        {
            Console.WriteLine("PAY MONEY");
            String svalue;
            decimal value = 0;
            Console.WriteLine("-----------------------------------");
            do
            {
                Console.Write("AMOUNT TO PAY TO USER:");
                svalue = Console.ReadLine();
                try
                {
                    value = decimal.Parse(svalue);
                }
                catch (Exception e)
                {
                }
                if (value <= 0)
                {
                    Console.WriteLine("The money need to be greater than 0.");
                }
                else
                {
                    break;
                }
            } while (true);
            //Pay money value need to be < 0
            value = -value;
            GrdResultBase result = GrdManager.ChargeMoney(username, value);
            if (result.error != 0)
            {
                printError(result);
            }
            else
            {
                Console.WriteLine("PAY SUCCESSFULLY!");
                pauseScreen();
            }
        }
        static void listTransactions()
        {
            int pageSize = 10;
            int pageIndex = 0;
            GrdResult<int> countResult = GrdManager.GetTransactionCount(username);
            if (countResult.error != 0)
            {
                printError(countResult);
            }
            else
            {
                do
                {
                    clearScreen();
                    Console.WriteLine("TRANSACTIONS");
                    Console.WriteLine("*********************************************************");
                    GrdResult<GrdTransaction[]> trans = GrdManager.GetTransactions(username, pageIndex * pageSize, pageSize);
                    if (trans.error == 0)
                    {
                        foreach (GrdTransaction tran in trans.data)
                        {
                            Console.WriteLine("------------------------------------------------------------");
                            Console.WriteLine("tx:" + tran.tx + "");
                            Console.WriteLine("time:" + tran.getTime().ToString() + "");
                            Console.WriteLine("from:" + tran.from + "");
                            Console.WriteLine("to:" + tran.to + "");
                            Console.WriteLine("amount:" + tran.amount.ToString() + "");
                            Console.WriteLine("type:" + (tran.transtype == TransactionType.Internal ? "Internal" : "External") + "");
                            Console.WriteLine("status:" + (tran.status == TransactionStatus.Success ? "Success" : (tran.status == TransactionStatus.Pending ? "Pending" : "Error")) + "");
                            Console.WriteLine("------------------------------------------------------------");
                        }

                        Console.WriteLine("*********************************************************");
                        int pageCount = (int)Math.Ceiling((double)countResult.data / pageSize);
                        Console.WriteLine("Page:" + (pageIndex + 1) + "/" + pageCount + "| Next:1-Prev:2-Exit:10");
                        Console.Write("YOUR CHOISE:");
                        userSelect = Console.ReadLine();
                        if (userSelect.Equals("2"))
                        {
                            if (pageIndex > 0)
                            {
                                pageIndex--;
                            }
                        }
                        else if (userSelect.Equals("1"))
                        {
                            if (pageIndex < pageCount - 1)
                            {
                                pageIndex++;
                            }
                        }
                        else if (userSelect.Equals("10"))
                        {
                            return;
                        }
                    }
                    else
                    {
                        printError(trans);
                    }

                } while (true);
            }
        }
        static void accountInfo()
        {
            while (true)
            {
                clearScreen();
                GrdResult<GrdAccountInfo> result = GrdManager.AccountBalance(username);
                Console.WriteLine("ACCOUNT INFORMATION");
                Console.WriteLine("-----------------------------------");
                if (result.error != 0)
                {
                    printError(result);
                }
                else
                {
                    Console.WriteLine("Username:" + username + "");
                    Console.WriteLine("Wallet address:" + result.data.address + "");
                    Console.WriteLine("Balance:" + result.data.balance.ToString() + "");
                }
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("TO DO");
                Console.WriteLine("1.Transfer money to other address (use for user).");
                Console.WriteLine("2.Charge from this account (use for game action).");
                Console.WriteLine("3.Pay money to this account(use for game action).");
                Console.WriteLine("4.List transactions");
                Console.WriteLine("5.Refresh.");
                Console.WriteLine("10.Go back...");
                Console.WriteLine("-----------------------------------");
                Console.Write("YOUR CHOISE:");
                userSelect = Console.ReadLine();
                if (userSelect.Equals("1"))
                {
                    transfer();
                }
                else if (userSelect.Equals("2"))
                {
                    chargeMoney();
                }
                else if (userSelect.Equals("3"))
                {
                    payMoney();
                }
                else if (userSelect.Equals("4"))
                {
                    listTransactions();
                }
                else if (userSelect.Equals("10"))
                {
                    return;
                }
            }
        }
        /*END ACCOUNT TEST*/
        /*SCRIPT SERVER API*/

        static void random09_history()
        {
            clearScreen();
            Console.WriteLine("RANDOM 1-9 HISTORY");
            Console.WriteLine("*********************************************************");
            GrdResult<GrdSessionData[]> result = GrdManager.GetUserSessionData(username, "GAME-9", "rand", 0, 20);
            if (result.error == 0)
            {
                Console.WriteLine("+-TIME-------------------------+---SELECT--+----RESULT----+-----MONEY-----+");
                foreach (GrdSessionData dt in result.data)
                {
                    if (dt.values.ContainsKey("rand"))
                    {
                        String value = dt.values["rand"];
                        String yourNumber = "";
                        String randNumber = "";
                        String money;
                        int ipos = value.IndexOf(",");
                        if (ipos > 0)
                        {
                            yourNumber = value.Substring(0, ipos);
                            value = value.Substring(ipos + 1);
                        }
                        ipos = value.IndexOf(",");
                        if (ipos > 0)
                        {
                            randNumber = value.Substring(0, ipos);
                            value = value.Substring(ipos + 1);
                        }
                        money = value;
                        Console.WriteLine("|" + formatField(dt.getTime().ToString(), 30) + "|" + formatField(yourNumber, 11) + "|" + formatField(randNumber, 14) + "|" + formatField(money, 15) + "|");
                        Console.WriteLine("+------------------------------+-----------+--------------+---------------+");
                    }
                }
                pauseScreen();
            }
            else
            {
                printError(result);
            }
        }
        static void random09Game()
        {
            String svalue;
            int number;
            decimal value;
            Console.WriteLine("RANDOM 1-9 GAME");
            while (true)
            {
                do
                {
                    clearScreen();
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("1-9:YOUR LUCKY NUMBER.");
                    Console.WriteLine("10. LEADER BOARD.");
                    Console.WriteLine("11. HISTORY.");
                    Console.WriteLine("100. EXIT.");
                    Console.WriteLine("-----------------------------------");
                    Console.Write("SELECT:");
                    svalue = Console.ReadLine();
                    if (svalue.Length > 0)
                    {
                        number = int.Parse(svalue);
                        if (number >= 1 && number <= 9)
                        {
                            break;
                        }
                        if (number == 10)
                        {
                            clearScreen();
                            game_leaderboard("RANDOM 1-9 GAME LEADER BOARD", "random9_score");
                        }
                        if (number == 11)
                        {
                            random09_history();
                        }
                        if (number == 100)
                        {
                            return;
                        }
                    }
                } while (true);
                do
                {
                    Console.Write("BET:");
                    svalue = Console.ReadLine();
                    try
                    {
                        value = decimal.Parse(svalue);
                        if (value > 0)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    Console.WriteLine("Bet must be greater than 0!");
                } while (true);
                GrdCustomResult result = GrdManager.CallServerScript(username, "testscript", "random9", new Object[] { number, value });
                if (result.error != 0)
                {
                    printError(result);
                }
                else
                {
                    //Server response an array
                    List<object> jsonArray = (List<object>)result.data;
                    try
                    {
                        if (jsonArray[0].ToString().Equals("0"))
                        {
                            Console.WriteLine("SELECT:" + jsonArray[2].ToString() + ",RESULT:" + jsonArray[1].ToString() + "");
                            Console.WriteLine((double.Parse(jsonArray[3].ToString()) > 0 ? "WIN:" : "LOSE:") + jsonArray[3].ToString() + "");
                        }
                        else
                        {
                            Console.WriteLine(jsonArray[1].ToString());//Message in game
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO Auto-generated catch block
                    }
                    pauseScreen();
                }
            }
        }

        static void lowhighgame_history()
        {
            clearScreen();
            Console.WriteLine("LOW-HIGH GAME HISTORY");
            Console.WriteLine("*********************************************************");
            GrdResult<GrdSessionData[]> result = GrdManager.GetUserSessionData(username, "LOWHIGHGAME", "result", 0, 20);
            if (result.error == 0)
            {
                Console.WriteLine("+-TIME-------------------------+-----CARD-----+---SELECT--+----RESULT----+-----MONEY-----+");
                foreach (GrdSessionData dt in result.data)
                {
                    //Contains the rand key
                    if (dt.values.ContainsKey("result"))
                    {
                        String value = dt.values["result"];
                        //Read the array result
                        List<object> jValue;
                        try
                        {
                            jValue = (List<object>)Json.Deserialize(value);
                            bool islow = jValue[0].ToString() == "1";
                            string yourNumber = jValue[1].ToString();
                            string randNumber = jValue[2].ToString();
                            double money = double.Parse(jValue[3].ToString());
                            //
                            Console.WriteLine("|" + formatField(dt.getTime().ToString(), 30) + "|" + formatField(yourNumber + "", 14) + "|" + formatField((islow ? "LOW" : "HIGH"), 11) + "|" + formatField(randNumber + "", 14) + "|" + formatField(money + "", 15) + "|");
                            Console.WriteLine("+------------------------------+--------------+-----------+--------------+---------------+");
                        }
                        catch (Exception e)
                        {
                            // TODO Auto-generated catch block
                        }

                    }
                }
                pauseScreen();
            }
            else
            {
                printError(result);
            }
        }
        static void highlowgame()
        {
            String svalue;
            double number;
            decimal value;
            bool islow = false;
            int LOW = 3;
            int HIGH = 13;
            while (true)
            {
                Random ramdom = new Random(System.DateTime.Now.Millisecond);
                clearScreen();
                Console.WriteLine("HIGH-LOW GAME");
                Console.WriteLine("-----------------------------------");
                number = Math.Round(ramdom.NextDouble() * (HIGH - LOW)) + LOW;
                Console.WriteLine("1. LOW: 2 To " + number + "(Bet Rate:" + ((14 - number) / (number - 2)) + "/1)");
                Console.WriteLine("2. HIGH: " + number + " To 14(Bet Rate:" + ((number - 2) / (14 - number)) + "/1)");
                Console.WriteLine("3. RANDOM NEXT");
                Console.WriteLine("4. LEADER BOARD");
                Console.WriteLine("5. HISTORY");
                Console.WriteLine("10. EXIT...");
                Console.Write("SELECT:");
                svalue = Console.ReadLine();
                if ((svalue.Equals("1")) || (svalue.Equals("2")))
                {
                    islow = svalue.Equals("1");
                }
                else if (svalue.Equals("4"))
                {
                    clearScreen();
                    game_leaderboard("LOW HIGH GAME LEADER BOARD", "lowhighgame_score");
                    continue;
                }
                else if (svalue.Equals("5"))
                {
                    clearScreen();
                    lowhighgame_history();
                    continue;
                }
                else if (svalue.Equals("10"))
                {
                    return;
                }
                else
                {
                    continue;
                }
                do
                {
                    Console.Write("BET:");
                    svalue = Console.ReadLine();
                    try
                    {
                        value = decimal.Parse(svalue);
                        if (value > 0)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    Console.WriteLine("Bet must be greater than 0!");
                } while (true);
                GrdCustomResult result = GrdManager.CallServerScript(username, "testscript", "lowhighgame", new Object[] { islow ? "1" : "0", number, value });
                if (result.error != 0)
                {
                    printError(result);
                }
                else
                {
                    //Server response an array
                    List<object> jsonArray = (List<object>)result.data;
                    try
                    {
                        if (jsonArray[0].ToString() == "0")
                        {
                            Dictionary<string, object> jobj = (Dictionary<string, object>)jsonArray[1];
                            int symbol = int.Parse(jobj["symbol"].ToString());
                            double money = double.Parse(jsonArray[2].ToString());
                            Console.WriteLine("NUMBER:" + number + ",SELECT:" + (islow ? "LOW" : "HIGH") + ",RESULT:" + symbol + "");
                            Console.WriteLine((money > 0 ? "WIN:" : "LOSE:") + money + "");
                        }
                        else
                        {
                            Console.WriteLine(jsonArray[1].ToString());//Message in game
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO Auto-generated catch block
                    }
                    pauseScreen();
                }
            }
        }
        static void scriptServerMenu()
        {
            do
            {
                clearScreen();
                Console.WriteLine("SCRIPT SERVER");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1. RANDOM 1-9 GAME.");
                Console.WriteLine("2. HIGH LOW GAME.");
                Console.WriteLine("10. Exit.");
                Console.WriteLine("-----------------------------------");
                Console.Write("SELECT:");
                userSelect = Console.ReadLine();
                if (userSelect.Equals("1"))
                {
                    random09Game();
                }
                else if (userSelect.Equals("2"))
                {
                    highlowgame();
                }
                else if (userSelect.Equals("10"))
                {
                    break;
                }
            } while (true);
        }
        static void setScore()
        {
            String value;
            double score;
            Console.WriteLine("-----------------------------------");
            Console.Write("SET USER SCORE:");
            value = Console.ReadLine();
            score = Double.Parse(value);
            GrdResultBase result = GrdManager.SaveUserScore(username, scoreType, score);
            if (result.error != 0)
            {
                printError(result);
            }
            else
            {
                Console.WriteLine("SAVE SCORE SUCCESSFULLY!");
                pauseScreen();
            }
        }
        static void increaseScore()
        {
            String value;
            double score;
            Console.WriteLine("-----------------------------------");
            Console.Write("INCREASE USER SCORE:");
            value = Console.ReadLine();
            score = Double.Parse(value);
            GrdResultBase result = GrdManager.IncreaseUserScore(username, scoreType, score);
            if (result.error != 0)
            {
                printError(result);
            }
            else
            {
                Console.WriteLine("INCREASE SCORE SUCCESSFULLY!");
                pauseScreen();
            }
        }

        static void ScoreApiTest()
        {
            while (true)
            {
                GrdResult<GrdLeaderBoard> result = GrdManager.GetUserScoreRank(username, scoreType);
                if (result.error == 0)
                {
                    clearScreen();
                    Console.WriteLine("SCORE API TEST");
                    Console.WriteLine("Score type test:" + scoreType + "");
                    Console.WriteLine("User score:" + result.data.score + "");
                    Console.WriteLine("User rank:" + result.data.rank + "");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("1. Set score");
                    Console.WriteLine("2. Increase score");
                    Console.WriteLine("3. Leader board");
                    Console.WriteLine("10.Exit.");
                    Console.WriteLine("-----------------------------------");
                    Console.Write("YOUR CHOISE:");
                    userSelect = Console.ReadLine();
                    if (userSelect.Equals("1"))
                    {
                        setScore();
                    }
                    else if (userSelect.Equals("2"))
                    {
                        increaseScore();
                    }
                    else if (userSelect.Equals("3"))
                    {
                        game_leaderboard("GAME LEADER BOARD SCORETYPE:" + scoreType, scoreType);
                    }
                    else if (userSelect.Equals("10"))
                    {
                        return;
                    }
                }
                else
                {
                    printError(result);
                    return;
                }

            }
        }
        static void testMenu()
        {
            while (true)
            {
                clearScreen();
                Console.WriteLine("SELECT MENU");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1.Account API.");
                Console.WriteLine("2.Scores API.");
                Console.WriteLine("3.Script Server-OAPI.");
                Console.WriteLine("10.Exit.");
                Console.WriteLine("-----------------------------------");
                Console.Write("YOUR CHOISE:");
                userSelect = Console.ReadLine();
                if (userSelect.Equals("1"))
                {
                    accountInfo();
                }
                else if (userSelect.Equals("2"))
                {
                    ScoreApiTest();
                }
                else if (userSelect.Equals("3"))
                {
                    scriptServerMenu();
                }
                else if (userSelect.Equals("10"))
                {
                    return;
                }
            }
        }

        public static void Main(String[] args)
        {
            const String appId = "6e672e888487bd8346b946a715c74890077dc332";
            const String secret = "acc3e0404646c57502b480dc052c4fe15878a7ab84fb43402106c575658472faf7e9050c92a851b0016442ab604b0488aab3e67537fcfda3650ad6cfd43f7974";
            GrdManager.Init(appId, secret);
            clearScreen();
            Console.Write("USER NAME:");
            username = Console.ReadLine();
            if (username.Length == 0)
            {
                return;
            }
            GrdManager.AccountBalance(username);//create user if is new user
            testMenu();
        }

    }
}
