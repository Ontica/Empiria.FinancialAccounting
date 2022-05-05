/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : SectorDto                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for economic sectors.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Data transfer object for economic sectors.</summary>
  public class SectorDto {

    internal SectorDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string FullName {
      get; internal set;
    }


    public string Code {
      get; internal set;
    }

  }  //class SectorDto

}  // namespace Empiria.FinancialAccounting.Adapters
