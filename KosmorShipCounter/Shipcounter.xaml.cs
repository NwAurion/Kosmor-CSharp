using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace KosmorShipCounter
{
    /// <summary>
    /// Interaktionslogik für Shipcounter.xaml
    /// </summary>
    public partial class Shipcounter : UserControl
    {
        int[] totalCombatPower = new int[12];
        int[] count = new int[12];
        int[] total = new int[11];

        int[] COMBAT_POWER = new int[] { 9, 13, 19, 41, 65, 116, 188, 310, 528, 916, 3306 };

        String[] text = { " probes (H1) with ", " orbital gliders (H2) with ", " space fighters (H3) with ", " interceptors (H4) with ", " frigates (H5) with ", " heavy frigates (H6) with ",
                          " cruisers (H7) with ", " heavy cruisers (H8) with ", " destroyers (H9) with ", " goliaths (H10) with ", " leviathans (H11) with "};


        public bool FromBattleReport { get; set; }
        static String house;
        List<String> owners = new List<String>();

        public Shipcounter()
        {
            InitializeComponent();
            checkBoxBattleReport.DataContext = this;
        }

        public String outputBuilder(String owner)
        {
            for (int i = 0; i < 11; i++)
            {
                totalCombatPower[i] = count[i] * COMBAT_POWER[i];
                totalCombatPower[11] = totalCombatPower[11] + totalCombatPower[i];
            }
            String outputString = "";
            for (int i = 0; i < 11; i++)
            {
                if (count[i] > 0)
                {
                    outputString = outputString + count[i] + text[i] + totalCombatPower[i] + " cp \n";
                }
            }
            outputString = owner + " has " + "\n" + outputString + "Total: "
                    + totalCombatPower[11] + " cp " + "\n\n";
            return outputString;
        }

        public void calculate()
        {
            for (int i = 0; i < 11; i++)
            {
                count[i] = 0;
                total[i] = 0;
                totalCombatPower[11] = 0;
            }

            postResultHere.Text = "";

            int totalLines = postShipListHere.LineCount;
            String[][] shipString = new String[totalLines][];
            int wasNoShip = 0;
            wasNoShip = gettingShipInfo(wasNoShip, postShipListHere.Text, totalLines, shipString);
            addShipsToOwners(wasNoShip, totalLines, shipString, owners);

            int shipType;

            if (FromBattleReport == false)
            {
                shipType = 0;
            }
            else
            {
                shipType = 1;
            }

            countShipsAndCombatPower(wasNoShip, totalLines, shipString, owners, shipType);


            for (int i = 0; i < 11; i++)
            {
                count[i] = total[i];
            }

            postResultHere.Text = postResultHere.Text
                    + outputBuilder("The house " + house);
        }

        private void addShipsToOwners(int wasNoShipCounter, int totalLines, String[][] shipInfo, List<String> owners)
        {
            for (int i = 0; i < totalLines - wasNoShipCounter; i++)
            {
                // Some ship type, like "orbital glider" consist of two words. Here, they get put together into one field
                if ((shipInfo.GetValue(i) as String[]).Length == 7)
                {
                    String[,] newShipString = new String[1, 6];
                    newShipString[0, 0] = shipInfo[i][0] + shipInfo[i][1];
                    shipInfo[i][0] = newShipString[0, 0];

                    // Moving the other info one field forward, to match ship types with a single word, like "goliath"
                    for (int j = 1; j < 5; j++)
                    {
                        shipInfo[i][j] = shipInfo[i][j + 1];
                    }
                }

                // Removing the trailing comma
                if (!FromBattleReport)
                    shipInfo[i][3] = shipInfo[i][3].Substring(0, shipInfo[i][3].Length - 1);

                // Adding the ships to their respective owners.
                if (!owners.Contains(shipInfo[i][3]))
                {
                    owners.Add(shipInfo[i][3]);
                }
            }
        }

        private void countShipsAndCombatPower(int wasNoShipCounter, int totalLines, String[][] shipInfo, List<String> owners, int shipType)
        {
            for (int ownerIterator = 0; ownerIterator < owners.Count; ownerIterator++)
            {
                for (int shipIterator = 0; shipIterator < totalLines - wasNoShipCounter; shipIterator++)
                {
                    if (shipInfo[shipIterator][3].Equals(owners.ElementAt(ownerIterator)))
                    {
                        for (int i = 0; i < 11; i++)
                        {
                            int ship = i + 1;
                            if (shipInfo[shipIterator][0].Contains("(H" + ship + ")"))
                            {
                                count[i]++;
                            }
                        }
                    }
                }
                // Creating the output and setting the text on the right side to it
                postResultHere.Text = postResultHere.Text + outputBuilder(owners[ownerIterator]);

                // Adding up the totals and resetting the other counters
                for (int j = 0; j < 11; j++)
                {
                    total[j] = total[j] + count[j];
                    count[j] = 0;
                    totalCombatPower[11] = 0;
                    count[11] = 0;
                }
            }
        }

        private int gettingShipInfo(int wasNoShipCount, String text, int totalLines, String[][] shipInfo)
        {
            for (int i = 0; i < totalLines; i++)
            { // Parsing every line
                int start = postShipListHere.GetCharacterIndexFromLineIndex(i); ;
                int end = postShipListHere.GetLineLength(i);

                // Getting a single line  form the text
                String line = text.Substring(start, end);
                line = line.Trim();

                // Counting the lines that do not contain ships
                string pattern = ".*(H[0-9]).*";
                if (!Regex.IsMatch(line, pattern))
                {
                    wasNoShipCount++; // Increase empty line counter
                    continue; // Next loop
                }

                // Splitting the line to get all the info
                char[] split = { ' ' };
                String[] ship = line.Split(split);
                shipInfo.SetValue(ship, i - wasNoShipCount);
                if (!FromBattleReport)
                {
                    house = shipInfo[i - wasNoShipCount][5];
                }
                else house = "";
            }
            return wasNoShipCount;
        }

        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            calculate();
        }
    }
}
