/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : AccountsListExcelExporter                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service used to export accounts lists to Microsoft Excel.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.AccountsLists.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Main service used to export accounts lists to Microsoft Excel.</summary>
  internal class AccountsListExcelExporter {

    private readonly FileTemplateConfig _templateConfig;

    private ExcelFile excelFile;

    public AccountsListExcelExporter(FileTemplateConfig templateConfig) {
      Assertion.Require(templateConfig, nameof(templateConfig));

      _templateConfig = templateConfig;
    }


    internal ExcelFile CreateExcelFile(AccountsListDto accountsList) {
      Assertion.Require(accountsList, nameof(accountsList));

      excelFile = new ExcelFile(_templateConfig);

      excelFile.Open();

      SetHeader();

      FillOut(accountsList);

      excelFile.Save();

      excelFile.Close();

      return excelFile;
    }



    #region Private methods

    private void FillOut(AccountsListDto list) {

      switch (list.UID) {
        case "ConciliacionDerivados":
          FillOut(list.Entries.Select(x => (ConciliacionDerivadosListItemDto) x).ToFixedList());
          break;

        case "SwapsCobertura":
          FillOut(list.Entries.Select(x => (SwapsCoberturaListItemDto) x).ToFixedList());
          break;

        case "DepreciacionActivoFijo":
          FillOut(list.Entries.Select(x => (DepreciacionActivoFijoListItemDto) x).ToFixedList());
          break;

        case "PrestamosInterbancarios":
          FillOut(list.Entries.Select(x => (PrestamosInterbancariosListItemDto) x).ToFixedList());
          break;

        default:
          throw new NotImplementedException($"Unrecognized accounts list UID {list.UID}.");
      }

    }


    private void FillOut(FixedList<ConciliacionDerivadosListItemDto> listItems) {
      int i = 5;

      foreach (var item in listItems) {
        excelFile.SetCell($"A{i}", item.AccountNumber);
        excelFile.SetCell($"B{i}", item.AccountName);
        excelFile.SetCell($"C{i}", item.StartDate);
        excelFile.SetCell($"D{i}", item.EndDate);

        i++;
      }
    }

    private void FillOut(FixedList<DepreciacionActivoFijoListItemDto> listItems) {
      int i = 5;

      foreach (var item in listItems) {
        excelFile.SetCell($"A{i}", item.NumeroDelegacion);
        excelFile.SetCell($"B{i}", item.Delegacion);
        excelFile.SetCell($"C{i}", item.AuxiliarHistorico);
        excelFile.SetCell($"D{i}", item.NumeroInventario);
        excelFile.SetCell($"E{i}", item.TipoActivoFijoName);
        excelFile.SetCell($"F{i}", item.AuxiliarHistoricoNombre);
        excelFile.SetCell($"G{i}", item.FechaAdquisicion);
        excelFile.SetCell($"H{i}", item.FechaInicioDepreciacion);
        excelFile.SetCell($"I{i}", item.MesesDepreciacion);
        excelFile.SetCell($"J{i}", item.AuxiliarRevaluacion);
        excelFile.SetCell($"K{i}", item.AuxiliarRevaluacionNombre);

        i++;
      }
    }


    private void FillOut(FixedList<PrestamosInterbancariosListItemDto> listItems) {
      int i = 5;

      foreach (var item in listItems) {
        excelFile.SetCell($"A{i}", item.PrestamoName);
        excelFile.SetCell($"B{i}", item.SubledgerAccountNumber);
        excelFile.SetCell($"C{i}", item.SubledgerAccountName);
        excelFile.SetCell($"D{i}", item.SectorCode);
        excelFile.SetCell($"E{i}", item.CurrencyCode);
        excelFile.SetCell($"F{i}", item.Vencimiento);

        i++;
      }
    }


    private void FillOut(FixedList<SwapsCoberturaListItemDto> listItems) {
      int i = 5;

      foreach (var item in listItems) {
        excelFile.SetCell($"A{i}", item.SubledgerAccountNumber);
        excelFile.SetCell($"B{i}", item.SubledgerAccountName);
        excelFile.SetCell($"C{i}", item.Classification);
        excelFile.SetCell($"D{i}", item.StartDate);
        excelFile.SetCell($"E{i}", item.EndDate);

        i++;
      }
    }


    private void SetHeader() {
      excelFile.SetCell($"A2", _templateConfig.Title);

      var subTitle = $"Fecha de consulta {DateTime.Now:dd/MMM/yyyy} ";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods


  } // class AccountsListExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
