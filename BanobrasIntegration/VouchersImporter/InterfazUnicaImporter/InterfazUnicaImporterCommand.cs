/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Command payload                      *
*  Type     : InterfazUnicaImporterCommand                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Command used to import vouchers from an InterfazUnica data structure.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>Command used to import vouchers from an InterfazUnica data structure.</summary>
  public class InterfazUnicaImporterCommand {

    public string ImportationRuleUID {
      get; set;
    } = string.Empty;


    public EncabezadoDto[] ENCABEZADOS {
      get; set;
    } = new EncabezadoDto[0];


  }  // class InterfazUnicaImporterCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters
