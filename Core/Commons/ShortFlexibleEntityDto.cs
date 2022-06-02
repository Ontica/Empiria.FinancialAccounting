/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : ShortFlexibleEntityDto                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO used as a wild card to transfer some minimal entity data.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  /// <summary>DTO used as a wild card to transfer some minimal entity data.</summary>
  public class ShortFlexibleEntityDto {

    public ShortFlexibleEntityDto() {
      // no-op
    }

    public int? Id {
      get;
      internal set;
    } = null;


    public string UID {
      get; set;
    }

    public string Code {
      get; set;
    }

    public string Number {
      get; set;
    }

    public string Name {
      get; set;
    }

    public string FullName {
      get; set;
    }

  }  // class ShortFlexibleEntityDto


}  // namespace Empiria.FinancialAccounting
