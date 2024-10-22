﻿using System;

namespace OrderMatcher.Types.Serializers
{
    public class MatchingEngineResultSerializer : Serializer
    {
        private static readonly short version;
        private static readonly int messageLengthOffset;
        private static readonly int messageTypeOffset;
        private static readonly int versionOffset;
        private static readonly int orderIdOffset;
        private static readonly int resultOffset;
        private static readonly int timestampOffset;

        private static readonly int sizeOfMessageLength;
        private static readonly int sizeOfMessage;
        private static readonly int sizeOfVersion;
        private static readonly int sizeOfMessagetType;
        private static readonly int sizeOfOrderId;
        private static readonly int sizeOfResult;
        private static readonly int sizeOfTimestamp;

        public static int MessageSize => sizeOfMessage;

        static MatchingEngineResultSerializer()
        {
            sizeOfMessageLength = sizeof(int);
            sizeOfMessagetType = sizeof(MessageType);
            sizeOfVersion = sizeof(short);
            sizeOfOrderId = sizeof(ulong);
            sizeOfResult = sizeof(byte);
            sizeOfTimestamp = sizeof(long);
            version = 1;

            messageLengthOffset = 0;
            messageTypeOffset = messageLengthOffset + sizeOfMessageLength;
            versionOffset = messageTypeOffset + sizeOfMessagetType;
            orderIdOffset = versionOffset + sizeOfVersion;
            resultOffset = orderIdOffset + sizeOfOrderId;
            timestampOffset = resultOffset + sizeOfResult;
            sizeOfMessage = timestampOffset + sizeOfTimestamp;
        }

        public static void Serialize(MatchingEngineResult matchingEngineResult, Span<byte> bytes)
        {
            if (matchingEngineResult == null)
                throw new ArgumentNullException(nameof(matchingEngineResult));

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            Serialize(matchingEngineResult.OrderId, matchingEngineResult.Result, matchingEngineResult.Timestamp, bytes);
        }

        public static void Serialize(ulong orderId, OrderMatchingResult result, long timeStamp, Span<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length < sizeOfMessage)
                throw new ArgumentException(Constant.INVALID_SIZE, nameof(bytes));

            Write(bytes[messageLengthOffset..], sizeOfMessage);
            bytes[messageTypeOffset] = (byte)MessageType.OrderMatchingResult;
            Write(bytes[versionOffset..], version);
            Write(bytes[orderIdOffset..], orderId);
            Write(bytes[resultOffset..], (byte)result);
            Write(bytes[timestampOffset..], timeStamp);
        }

        public static MatchingEngineResult Deserialize(ReadOnlySpan<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != sizeOfMessage)
                throw new Exception("OrderMatchingResult Message must be of Size : " + sizeOfMessage);

            var messageType = (MessageType)bytes[messageTypeOffset];

            if (messageType != MessageType.OrderMatchingResult)
                throw new Exception(Constant.INVALID_MESSAGE);

            var version = BitConverter.ToInt16(bytes[versionOffset..]);

            if (version != MatchingEngineResultSerializer.version)
                throw new Exception(Constant.INVALID_VERSION);

            var result = new MatchingEngineResult();
            result.OrderId = BitConverter.ToUInt64(bytes[orderIdOffset..]);
            result.Result = (OrderMatchingResult)bytes[resultOffset];
            result.Timestamp = BitConverter.ToInt64(bytes[timestampOffset..]);
            return result;
        }
    }
}
