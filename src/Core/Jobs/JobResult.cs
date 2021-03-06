using System;

namespace Foundatio.Jobs {
    public class JobResult {
        public bool IsCancelled { get; set; }
        public Exception Error { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public static readonly JobResult None = new JobResult {
            IsSuccess = true,
            Message = "Nothing to do."
        };

        public static readonly JobResult Cancelled = new JobResult {
            IsCancelled = true
        };

        public static readonly JobResult Success = new JobResult {
            IsSuccess = true
        };

        public static JobResult FromException(Exception exception, string message = null) {
            return new JobResult {
                Error = exception,
                IsSuccess = false,
                Message = message ?? exception.Message
            };
        }

        public static JobResult FromException(Exception exception, string format, params object[] args) {
            return new JobResult {
                Error = exception,
                IsSuccess = false,
                Message = String.Format(format, args)
            };
        }

        public static JobResult SuccessWithMessage(string message) {
            return new JobResult {
                IsSuccess = true,
                Message = message
            };
        }

        public static JobResult SuccessWithMessage(string format, params object[] args) {
            return new JobResult {
                IsSuccess = true,
                Message = String.Format(format, args)
            };
        }

        public static JobResult FailedWithMessage(string message) {
            return new JobResult {
                IsSuccess = false,
                Message = message
            };
        }

        public static JobResult FailedWithMessage(string format, params object[] args) {
            return new JobResult {
                IsSuccess = false,
                Message = String.Format(format, args)
            };
        }
    }
}