/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : Information Holder                    *
*  Type     : ExcelFile                                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for Microsoft Excel files.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Office;
using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides edition services for Microsoft Excel files.</summary>
  internal class ExcelFile {

    #region Fields

    private readonly FileTemplateConfig _templateConfig;

    private Spreadsheet _excel;

    #endregion Fields

    public ExcelFile(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }

    #region Properties

    public string Url {
      get {
        return $"{FileTemplateConfig.GeneratedFilesBaseUrl}/{FileInfo.Name}";
      }
    }


    public FileInfo FileInfo {
      get; private set;
    }

    #endregion Properties

    #region Methods

    internal void Open() {
      _excel = Spreadsheet.Open(_templateConfig.TemplateFullPath);
    }

    internal void Close() {
      if (_excel != null) {
        _excel.Close();
      }
    }

    internal void Save() {
      if (_excel != null) {
        var path = _templateConfig.NewFilePath();
        _excel.SaveAs(path);
        this.FileInfo = new FileInfo(path);
      }
    }

    internal void IndentCell(string cell, int indent) {
      if (_excel != null) {
        _excel.IndentCell(cell, indent);
      }
    }

    public void RemoveColumn(string column) {
      _excel.RemoveColumn(column);
    }

    internal void SetCell(string cell, string value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    internal void SetCell(string cell, decimal value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    internal void SetCell(string cell, int value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    internal void SetCell(string cell, DateTime value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    internal void SetCellStyleLineThrough(string cell) {
      if (_excel != null) {
        _excel.SetCellStyle(Style.LineThrough, cell);
      }
    }

    internal void SetRowStyleBold(int rowIndex) {
      if (_excel != null) {
        _excel.SetRowStyle(Style.Bold, rowIndex);
      }
    }

    internal void SetCellFontColorStyle(string cell, System.Drawing.Color color) {
      if (_excel != null) {
        _excel.SetFontColor(Style.FontColor, cell, color);
      }
    }

    internal void SetRowFontColorStyle(int rowIndex, System.Drawing.Color color) {
      if (_excel != null) {
        _excel.SetRowFontColor(Style.FontColor, rowIndex, color);
      }
    }

    internal void SetCellBackgroundStyle(string cell, DocumentFormat.OpenXml.Spreadsheet.PatternValues color) {
      if (_excel != null) {
        _excel.SetCellBackgroundStyle(Style.BackgroundColor, cell, color);
      }
    }

    internal void SetRowBackgroundStyle(int rowIndex, DocumentFormat.OpenXml.Spreadsheet.PatternValues color) {
      if (_excel != null) {
        _excel.SetRowBackgroundStyle(Style.BackgroundColor, rowIndex, color);
      }
    }

    internal FileReportDto ToFileReportDto() {
      return new FileReportDto(FileType.Excel, this.Url);
    }

    #endregion Methods

  }  // class ExcelFile

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
