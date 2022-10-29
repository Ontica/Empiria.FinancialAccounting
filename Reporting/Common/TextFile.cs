/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Text File Exporters                   *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : Information Holder                    *
*  Type     : TextFile                                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for text-based files.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using System.IO;
using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Text {

  /// <summary>Provides edition services for text-based files.</summary>
  internal class TextFile {

    private readonly List<string> _textLines = new List<string>(256);

    #region Fields

    #endregion Fields

    internal TextFile(string baseFileName) {
      Assertion.Require(baseFileName, "baseFileName");

      this.BaseFileName = baseFileName;
    }

    #region Properties

    public string BaseFileName {
      get; private set;
    }


    public FileInfo FileInfo {
      get; private set;
    }


    public FixedList<string> TextLines {
      get {
        return _textLines.ToFixedList();
      }
    }


    public string Url {
      get {
        if (FileInfo != null) {
          return $"{FileTemplateConfig.GeneratedFilesBaseUrl}/{FileInfo.Name}";
        } else {
          return $"{FileTemplateConfig.GeneratedFilesBaseUrl}/unavailable_file.txt";
        }
      }
    }


    #endregion Properties

    #region Methods


    internal void AppendLine(string textLine) {
      Assertion.Require(textLine, "textLine");

      _textLines.Add(textLine);
    }


    internal void AppendLines(FixedList<string> textLines) {
      Assertion.Require(textLines, "textLines");

      _textLines.AddRange(textLines);
    }


    private string BuildFilePath() {
      var copyFileName = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss-") + this.BaseFileName;

      return Path.Combine(FileTemplateConfig.GenerationStoragePath, copyFileName);
    }


    internal void Save() {
      var path = BuildFilePath();

      File.WriteAllLines(path, _textLines.ToArray());

      this.FileInfo = new FileInfo(path);
    }


    internal FileReportDto ToFileReportDto() {
      return new FileReportDto(FileType.Text, this.Url);
    }


    #endregion Methods

  }  // class TextFile

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Text
