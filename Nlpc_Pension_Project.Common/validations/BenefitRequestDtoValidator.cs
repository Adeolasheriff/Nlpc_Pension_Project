//using FluentValidation;
//using Microsoft.EntityFrameworkCore;
//using Nlpc_Pension_Project.Application.Dtos;
//using Nlpc_Pension_Project.Infrastructure.AppDbContext;

//public class BenefitRequestDtoValidator : AbstractValidator<BenefitRequestDto>
//{
//    public BenefitRequestDtoValidator(ApplicationDbContext context)
//    {
//        RuleFor(x => x.MemberId)
//            .GreaterThan(0).WithMessage("Member ID must be greater than 0.")
//            .Must((memberId) => // Changed from MustAsync to Must
//            {
//                var member = context.Members
//                   .Include(m => m.Contributions)
//                    .FirstOrDefault(m => m.Id == memberId && !m.IsDeleted);

//                if (member == null)
//                    return false;

//                var totalMonths = member.Contributions
//                    .Select(c => new DateTime(c.ContributionDate.Year, c.ContributionDate.Month, 1))
//                    .Distinct()
//                    .Count();

//                return totalMonths >= 60;
//            })
//            .WithMessage("Member must have contributed for at least 60 distinct months to be eligible.");
//    }
//}