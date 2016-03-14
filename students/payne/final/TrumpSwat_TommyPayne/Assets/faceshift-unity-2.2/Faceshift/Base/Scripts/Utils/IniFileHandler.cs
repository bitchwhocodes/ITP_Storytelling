/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;

/**
 * This is from here: http://bytes.com/topic/net/insights/797169-reading-parsing-ini-file-c
 * @brief class for handling ini files.
 */
public class IniFileHandler {

    private Hashtable keyPairs = new Hashtable();
    private String iniFilePath;

    private struct SectionPair {
        public String Section;
        public String Key;
    }

    /**
     * @brief Opens the INI file at the given path and enumerates the values in the IniParser.
     * @param iniPath full path to INI file.
     */
    public IniFileHandler(String iniPath) {
    
        TextReader iniFile = null;
        String strLine = null;
        String currentRoot = null;
        String[] keyPair = null;

        iniFilePath = iniPath;

        if (!File.Exists(iniPath)) {
            // Create it
            SaveSettings();
        }

        if (File.Exists(iniPath)) {
            try {
                iniFile = new StreamReader(iniPath);

                strLine = iniFile.ReadLine();

                while (strLine != null) {
                    strLine = strLine.Trim();

                    if (strLine != "") {
                        if (strLine.StartsWith("[") && strLine.EndsWith("]")) {
                            currentRoot = strLine.Substring(1, strLine.Length - 2);
                        } else {
                            keyPair = strLine.Split(new char[] { '=' }, 2);

                            SectionPair sectionPair;
                            String value = null;

                            if (currentRoot == null)
                                currentRoot = "ROOT";

                            sectionPair.Section = currentRoot;
                            sectionPair.Key = keyPair[0];

                            if (keyPair.Length > 1)
                                value = keyPair[1];

                            keyPairs.Add(sectionPair, value);
                        }
                    }

                    strLine = iniFile.ReadLine();
                }

            } catch (Exception ex) {
                throw ex;
            } finally {
                if (iniFile != null)
                    iniFile.Close();
            }
        } else {
            throw new FileNotFoundException("Unable to locate " + iniPath);
        }
    }

    /**
     * @param sectionName section name.
     * @param settingName key name.
     * @return the value for the given section, key pair.
     */
    public String GetSetting(String sectionName, String settingName) {
    
        SectionPair sectionPair;
        sectionPair.Section = sectionName;
        sectionPair.Key = settingName;

        return (String)keyPairs[sectionPair];
    }

    /**
     * @brief Enumerates all lines for given section.
     * @param sectionName section to enum.
     */
    public String[] EnumSection(String sectionName) {
    
        ArrayList tmpArray = new ArrayList();

        foreach (SectionPair pair in keyPairs.Keys) {
            if (pair.Section == sectionName)
                tmpArray.Add(pair.Key);
        }

        return (String[])tmpArray.ToArray(typeof(String));
    }

    /**
     * @brief Adds or replaces a setting to the table to be saved.
     * @param sectionName section to add under.
     * @param settingName key name to add.
     * @param settingValue value of key.
     */
    public void AddSetting(String sectionName, String settingName, String settingValue) {
    
        SectionPair sectionPair;
        sectionPair.Section = sectionName;
        sectionPair.Key = settingName;

        if (keyPairs.ContainsKey(sectionPair))
            keyPairs.Remove(sectionPair);

        keyPairs.Add(sectionPair, settingValue);
    }

    /**
     * @brief Adds or replaces a setting to the table to be saved with a null value.
     * @param sectionName section to add under.
     * @param settingName key name to add.
     */
    public void AddSetting(String sectionName, String settingName) {
    
        AddSetting(sectionName, settingName, null);
    }

    /**
     * @brief Remove a setting.
     * @param sectionName section to add under.
     * @param settingName key name to add.
     */
    public void DeleteSetting(String sectionName, String settingName) {
    
        SectionPair sectionPair;
        sectionPair.Section = sectionName;
        sectionPair.Key = settingName;

        if (keyPairs.ContainsKey(sectionPair))
            keyPairs.Remove(sectionPair);
    }

    /**
     * @brief save settings to new file.
     * @param newFilePath new file path.
     */
    public void SaveSettings(String newFilePath) {
    
        ArrayList sections = new ArrayList();
        String tmpValue = "";
        String strToSave = "";

        foreach (SectionPair sectionPair in keyPairs.Keys) {
            if (!sections.Contains(sectionPair.Section))
                sections.Add(sectionPair.Section);
        }

        foreach (String section in sections) {
            strToSave += ("[" + section + "]\r\n");

            foreach (SectionPair sectionPair in keyPairs.Keys) {
                if (sectionPair.Section == section) {
                    tmpValue = (String)keyPairs[sectionPair];

                    if (tmpValue != null)
                        tmpValue = "=" + tmpValue;

                    strToSave += (sectionPair.Key + tmpValue + "\r\n");
                }
            }

            strToSave += "\r\n";
        }

        try {
            TextWriter tw = new StreamWriter(newFilePath);
            tw.Write(strToSave);
            tw.Close();
        } catch (Exception ex) {
            throw ex;
        }
    }

    /**
     * @brief save settings back to ini file.
     */
    public void SaveSettings() {
    
        SaveSettings(iniFilePath);
    }
}

