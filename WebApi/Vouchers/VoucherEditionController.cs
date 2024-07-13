/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : VoucherEditionController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web API used to retrive accounting vouchers related data.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.Vouchers {

  /// <summary>Command web API used to retrive accounting vouchers related data.</summary>
  public class VoucherEditionController : WebApiController {

    #region Web Apis


    [HttpGet]
    [AllowAnonymous]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}")]
    public SingleObjectModel GetVoucher([FromUri] long voucherId) {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.GetVoucher(voucherId);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/entries/{voucherEntryId:int}")]
    public SingleObjectModel GetVoucherEntry([FromUri] long voucherId,
                                             [FromUri] long voucherEntryId) {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        VoucherEntryDto voucher = usecases.GetVoucherEntry(voucherId, voucherEntryId);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/get-copy-of-last-entry")]
    public SingleObjectModel GetCopyOfVoucherLastEntry([FromUri] long voucherId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherEntryDto copy = usecases.GetCopyOfLastEntry(voucherId);

        return new SingleObjectModel(base.Request, copy);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/assign-account/{standardAccountId:int}")]
    public SingleObjectModel AssignStandardAccountToVoucherLedger([FromUri] long voucherId,
                                                                  [FromUri] int standardAccountId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        LedgerAccountDto ledgerAccount = usecases.AssignVoucherLedgerStandardAccount(voucherId,
                                                                                     standardAccountId);

        return new SingleObjectModel(base.Request, ledgerAccount);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/entries")]
    public SingleObjectModel AppendVoucherEntry([FromUri] long voucherId,
                                                [FromBody] VoucherEntryFields fields) {
      base.RequireBody(fields);

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.AppendEntry(voucherId, fields);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/create-voucher")]
    public SingleObjectModel CreateVoucher([FromBody] VoucherFields fields) {
      base.RequireBody(fields);

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.CreateVoucher(fields);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}")]
    public NoDataModel DeleteVoucher([FromUri] long voucherId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        usecases.DeleteVoucher(voucherId);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/close")]
    public SingleObjectModel CloseVoucher([FromUri] long voucherId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.CloseVoucher(voucherId, false);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/entries/{voucherEntryId:int}")]
    public SingleObjectModel DeleteVoucherEntry([FromUri] long voucherId,
                                                [FromUri] long voucherEntryId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.DeleteEntry(voucherId, voucherEntryId);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/bulk-operation/{operationName}")]
    public SingleObjectModel ExecuteBulkOperation([FromUri] string operationName,
                                                  [FromBody] VoucherBulkOperationCommand command) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        var result = new VoucherBulkOperationResult();

        if (operationName == "close") {
          result.Message = usecases.BulkClose(command.Vouchers);

        } else if (operationName == "delete") {
          result.Message = usecases.BulkDelete(command.Vouchers);

        } else if (operationName == "send-to-supervisor") {
          result.Message = usecases.BulkSendToSupervisor(command.Vouchers);

        } else if (operationName == "print") {
          result = ExecuteBulkPrinting(command.Vouchers);

        } else if (operationName == "excel") {
          result = ExecuteBulkExportingToExcel(command.Vouchers);

        } else {
          throw Assertion.EnsureNoReachThisCode($"Unrecognized bulk operation name '{operationName}'.");
        }

        base.SetOperation(result.Message);

        return new SingleObjectModel(base.Request, result);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/send-to-supervisor")]
    public SingleObjectModel SendVoucherToSupervisor([FromUri] long voucherId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.SendVoucherToSupervisor(voucherId);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}")]
    public SingleObjectModel UpdateVoucher([FromUri] long voucherId,
                                           [FromBody] VoucherFields fields) {
      base.RequireBody(fields);

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.UpdateVoucher(voucherId, fields);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/update-concept")]
    public SingleObjectModel UpdateVoucherConcept([FromUri] long voucherId,
                                                  [FromBody] UpdateVoucherFields fields) {
      base.RequireBody(fields);

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.UpdateVoucherConcept(voucherId, fields);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/entries/{voucherEntryId:int}")]
    public SingleObjectModel UpdateVoucherEntry([FromUri] long voucherId,
                                                [FromUri] long voucherEntryId,
                                                [FromBody] VoucherEntryFields fields) {
      base.RequireBody(fields);

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.UpdateEntry(voucherId, voucherEntryId, fields);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/validate")]
    public CollectionModel ValidateVoucher([FromUri] long voucherId) {

      using (var usecases = VoucherEditionUseCases.UseCaseInteractor()) {
        FixedList<string> validationResult = usecases.ValidateVoucher(voucherId);

        return new CollectionModel(base.Request, validationResult);
      }
    }

    #endregion Web Apis

    #region Helpers

    private VoucherBulkOperationResult ExecuteBulkPrinting(int[] voucherIdsToPrint) {
      using (var usecases = VoucherUseCases.UseCaseInteractor()) {

        var result = new VoucherBulkOperationResult();

        FixedList<VoucherDto> vouchersToPrint = usecases.GetVouchers(voucherIdsToPrint);

        var exporter = new PdfExporterService();

        result.File = exporter.Export(vouchersToPrint);

        result.Message = $"Se generó el archivo de impresión con {voucherIdsToPrint.Length} pólizas.";

        return result;
      }
    }


    private VoucherBulkOperationResult ExecuteBulkExportingToExcel(int[] voucherIdsToExport) {
      using (var usecases = VoucherUseCases.UseCaseInteractor()) {

        var result = new VoucherBulkOperationResult();

        FixedList<VoucherDto> vouchersToExport = usecases.GetVouchersToExport(voucherIdsToExport);

        var exporter = new ExcelExporterService();

        FileDto excelFileDto = exporter.Export(vouchersToExport);

        result.Message = $"Se exportaron los movimientos de " +
                         $"{voucherIdsToExport.Length} pólizas a excel.";

        result.File = excelFileDto;

        return result;
      }
    }


    #endregion Helpers

  }  // class VoucherEditionController



  public class VoucherBulkOperationCommand {

    public int[] Vouchers {
      get;
      set;
    }

  }



  public class VoucherBulkOperationResult {

    internal VoucherBulkOperationResult() {
      // no-op
    }

    public string Message {
      get; internal set;
    }


    public FileDto File {
      get; internal set;
    }

  }  // class VoucherBulkOperationCommand

}  // namespace Empiria.FinancialAccounting.WebApi.Vouchers
