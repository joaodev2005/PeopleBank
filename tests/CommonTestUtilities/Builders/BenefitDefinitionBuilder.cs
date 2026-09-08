using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;

namespace PeopleBank.CommonTestUtilities.Builders;

public class BenefitDefinitionBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _companyId = Guid.NewGuid();
    private string _name = "Vale Refeição";
    private BenefitCategory _category = BenefitCategory.Meal;
    private decimal _monthlyAmount = 500m;
    private bool _active = true;

    public BenefitDefinitionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BenefitDefinitionBuilder WithCompanyId(Guid companyId)
    {
        _companyId = companyId;
        return this;
    }

    public BenefitDefinitionBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public BenefitDefinitionBuilder WithCategory(BenefitCategory category)
    {
        _category = category;
        return this;
    }

    public BenefitDefinitionBuilder WithMonthlyAmount(decimal amount)
    {
        _monthlyAmount = amount;
        return this;
    }

    public BenefitDefinitionBuilder Inactive()
    {
        _active = false;
        return this;
    }

    public BenefitDefinition Build()
    {
        var benefit = new BenefitDefinition(_companyId, _name, _category, _monthlyAmount);
        var idProp = typeof(BenefitDefinition).GetProperty(nameof(BenefitDefinition.Id));
        idProp!.SetValue(benefit, _id);

        var activeProp = typeof(BenefitDefinition).GetProperty(nameof(BenefitDefinition.Active));
        activeProp!.SetValue(benefit, _active);

        return benefit;
    }
}