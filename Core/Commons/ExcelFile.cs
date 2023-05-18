﻿/* Empiria Financial *****************************************************************************************
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

namespace Empiria.FinancialAccounting {

  /// <summary>Provides edition services for Microsoft Excel files.</summary>
  public class ExcelFile {

    #region Fields

    private readonly FileTemplateConfig _templateConfig;

    private Spreadsheet _excel;

    #endregion Fields

    public ExcelFile(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, " templateConfig");

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

    public void Open() {
      _excel = Spreadsheet.Open(_templateConfig.TemplateFullPath);
    }

    public void Close() {
      if (_excel != null) {
        _excel.Close();
      }
    }

    public void Save() {
      if (_excel != null) {
        var path = _templateConfig.NewFilePath();
        _excel.SaveAs(path);
        this.FileInfo = new FileInfo(path);
      }
    }

    public void IndentCell(string cell, int indent) {
      if (_excel != null) {
        _excel.IndentCell(cell, indent);
      }
    }

    public void RemoveColumn(string column) {
      _excel.RemoveColumn(column);
    }

    public void SetCell(string cell, string value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    public void SetCell(string cell, decimal value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    public void SetCell(string cell, int value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    public void SetCell(string cell, DateTime value) {
      if (_excel != null) {
        _excel.SetCell(cell, value);
      }
    }

    public void SetCellStyleLineThrough(string cell) {
      if (_excel != null) {
        _excel.SetCellStyle(Style.LineThrough, cell);
      }
    }

    public void SetRowStyleBold(int rowIndex) {
      if (_excel != null) {
        _excel.SetRowStyle(Style.Bold, rowIndex);
      }
    }

    public FileReportDto ToFileReportDto() {
      return new FileReportDto(FileType.Excel, this.Url);
    }

    #endregion Methods

  }  // class ExcelFile

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel