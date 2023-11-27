using AddressOwnershipTool.Commands;
using AddressOwnershipTool.Commands.Claim;
using AddressOwnershipTool.Commands.Distribute;
using AddressOwnershipTool.Commands.Validate;
using AddressOwnershipTool.Common;
using CommandLine;
using MediatR;

namespace AddressOwnershipTool
{
    public class App
    {
        private readonly IMediator _mediator;

        public App(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Run(string[] args)
        {
            //args = new string[] { "claim", "--walletname=testwallet", "--cirrus", "--destination=0x0D36390660dA0950c9d2BcA0e8868F3c4Fdb0eBA", "--walletpassword=Phoenix88" };
            //args = new string[] { "claim", "--help" };
            ICommand<Result> command = null;

            Parser
                .Default
                .ParseArguments<ClaimInstruction, DistributeInstruction, ValidateInstruction>(args)
                .MapResult(
                    (ClaimInstruction instruction) =>
                    {
                        command = instruction.ToCommand();
                        return 0;
                    },
                    (DistributeInstruction instruction) =>
                    {
                        command = instruction.ToCommand();
                        return 0;
                    },
                    (ValidateInstruction instruction) =>
                    {
                        command = instruction.ToCommand();
                        return 0;
                    },
                    errors => 1
                );

            if (command != null)
            {
                var result = await _mediator.Send(command);
                Console.WriteLine(result.Failure ? result.Message : "Finished");
            }
        }
    }
}
