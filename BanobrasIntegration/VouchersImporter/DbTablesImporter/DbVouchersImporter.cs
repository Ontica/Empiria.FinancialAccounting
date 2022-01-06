/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Singleton Controller                 *
*  Type     : DbVouchersImporter                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Performs voucher importation tasks from a central database.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Performs voucher importation tasks from a central database.</summary>
  internal class DbVouchersImporter {

    static private readonly DbVouchersImporter _singleton = new DbVouchersImporter();

    private ImportVouchersResult _importVouchersResult;

    private DbVouchersImporter() {
      // Singleton private constructor
    }

    static internal DbVouchersImporter Instance {
      get {
        return _singleton;
      }
    }


    #region Public members


    internal bool IsRunning {
      get;
      private set;
    }


    internal ImportVouchersResult GetImportVouchersResult() {
      if (!this.IsRunning) {
        UpdateImportVouchersResult();
      }
      return _importVouchersResult;
    }


    internal async Task Start(ImportVouchersCommand command) {
      if (this.IsRunning) {
        Assertion.AssertFail("DBVouchersImporter is running. Please stop it before call Start() method");
      }

      command.TryToCloseVouchers = true;     // ToDo: OOJJOO

      command.CanEditVoucherEntries = false;

      SetIsRunningFlag(true);

      await Task.Run(() => {
        try {
          ImportVouchers(command);

          SetIsRunningFlag(false);

        } catch (Exception e) {
          EmpiriaLog.Error(e);
          SetIsRunningFlag(false);
          throw;
        }
      });
    }


    private void ImportVouchers(ImportVouchersCommand command) {
      Assertion.AssertObject(command.ProcessOnly[0], "ImportationSet");

      List<Encabezado> encabezados = DbVouchersImporterDataService.GetEncabezados(command.ProcessOnly[0]);
      List<Movimiento> movimientos = DbVouchersImporterDataService.GetMovimientos(command.ProcessOnly[0]);

      EmpiriaLog.Info($"To be processed {encabezados.Count} at {DateTime.Now}.");

      DbVouchersImporterDataService.Reallocate(encabezados[0].IdSistema,
                                               encabezados[0].TipoContabilidad,
                                               encabezados[0].FechaAfectacion);

      if (encabezados[0].IdSistema == 24 || encabezados[0].IdSistema == 26) {   // YATLA PATCH
        command.TryToCloseVouchers = false;
      }

      foreach (var encabezado in encabezados) {
        if (!this.IsRunning) {
          EmpiriaLog.Info($"Voucher processing ends at {DateTime.Now}.");
          return;
        }

        var asList = new FixedList<Encabezado>(new Encabezado[1] { encabezado });

        var structurer = new DbVouchersStructurer(asList, movimientos.ToFixedList());

        FixedList<ToImportVoucher> toImport = structurer.GetToImportVouchersList();

        var voucherImporter = new VoucherImporter(command, toImport);

        foreach (ToImportVoucher item in toImport) {
          var voucher = voucherImporter.TryImportOne(item);

          long idVolante = DbVouchersImporterDataService.NextIdVolante();

          if (voucher != null) {
            DbVouchersImporterDataService.MergeVouchers(encabezado, idVolante, voucher.Id);
          } else {
            DbVouchersImporterDataService.StoreIssues(encabezado, item, idVolante);
          }
        }

        UpdateImportVouchersResult();
      }
    }


    internal void Stop() {
      SetIsRunningFlag(false);
    }

    #endregion Public members


    private void SetIsRunningFlag(bool isRunning) {
      this.IsRunning = isRunning;

      UpdateImportVouchersResult();
    }


    private void UpdateImportVouchersResult() {
      if (!this.IsRunning) {
        _importVouchersResult = DbVouchersImporterDataService.GetEncabezadosTotals();
      }

      _importVouchersResult.IsRunning = this.IsRunning;
    }


  }  // class DbVouchersImporter

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
