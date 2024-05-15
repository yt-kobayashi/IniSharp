using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace IniSharp
{
    internal sealed class NativeMethods
    {
        private NativeMethods() { }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, ExactSpelling = false)]
        internal static extern uint GetPrivateProfileString([MarshalAs(UnmanagedType.LPWStr), In] string lpAppName,
                                                            [MarshalAs(UnmanagedType.LPWStr), In] string lpKeyName,
                                                            [MarshalAs(UnmanagedType.LPWStr), In] string lpDefault,
                                                            [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpReturnString,
                                                            uint nSize,
                                                            [MarshalAs(UnmanagedType.LPWStr), In] string iniFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        internal static extern int WritePrivateProfileString([MarshalAs(UnmanagedType.LPWStr), In] string lpAppName,
                                                             [MarshalAs(UnmanagedType.LPWStr), In] string lpKeyName,
                                                             [MarshalAs(UnmanagedType.LPWStr), In] string lpString,
                                                             [MarshalAs(UnmanagedType.LPWStr), In] string lpFileName);
    }

    public class IniFile
    {
        private string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath">INIファイルパス</param>
        public IniFile(in string filePath)
        {
            string path = filePath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }

            SetFilePath(path);
        }

        /// <summary>
        /// INIファイルのパスを設定します
        /// </summary>
        /// <param name="filePath">INIファイルパス</param>
        /// <exception cref="ArgumentNullException">ファイルパスが空の場合</exception>
        public void SetFilePath(in string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException();
            }

            FilePath = filePath;
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="outputValue">出力値</param>
        /// <returns>true : 取得成功 / false : 取得失敗</returns>
        public bool TryGetValueOrDafault<T>(in string sectionName, in string keyName, out T? outputValue)
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException($"ファイルが見つかりませんでした。 ファイルパス:{FilePath}");
            }

            return IniFileAccessor.TryGetValueOrDafault(FilePath, sectionName, keyName, default, out outputValue);
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="defaultValue">初期値</param>
        /// <param name="outputValue">出力値</param>
        /// <returns>true : 取得成功 / false : 取得失敗</returns>
        public bool TryGetValueOrDafault<T>(in string sectionName, in string keyName, in T? defaultValue, out T? outputValue)
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException($"ファイルが見つかりませんでした。 ファイルパス:{FilePath}");
            }

            return IniFileAccessor.TryGetValueOrDafault(FilePath, sectionName, keyName, defaultValue, out outputValue);
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <returns>出力値</returns>
        public T? GetValueOrDefault<T>(in string sectionName, in string keyName)
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException($"ファイルが見つかりませんでした。 ファイルパス:{FilePath}");
            }

            return IniFileAccessor.GetValueOrDefault<T>(FilePath, sectionName, keyName);
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="defaultValue">初期値</param>
        /// <returns>出力値</returns>
        public T? GetValueOrDefault<T>(in string sectionName, in string keyName, in T? defaultValue)
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException($"ファイルが見つかりませんでした。 ファイルパス:{FilePath}");
            }

            return IniFileAccessor.GetValueOrDefault<T>(FilePath, sectionName, keyName, defaultValue);
        }

        /// <summary>
        /// INIファイルにデータを書き込みます
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="value">書き込む値</param>
        /// <returns>true : 書き込み成功 / false : 書き込み失敗</returns>
        public bool SetValue(in string sectionName, in string keyName, in string value)
        {
            return IniFileAccessor.SetValue(FilePath, sectionName, keyName, value);
        }
    }

    public static class IniFileAccessor
    {
        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="defaultValue">初期値</param>
        /// <param name="outputValue">出力値</param>
        /// <returns>true : 取得成功 / false : 取得失敗</returns>
        public static bool TryGetValueOrDafault<T>(in string filePath,
                                                   in string sectionName,
                                                   in string keyName,
                                                   in T? defaultValue,
                                                   out T? outputValue)
        {
            outputValue = defaultValue;

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) { return false; }

            var stringBuilder = new StringBuilder(1024);
            uint ret = NativeMethods.GetPrivateProfileString(sectionName, keyName, string.Empty, stringBuilder, Convert.ToUInt32(stringBuilder.Capacity), filePath);
            string? iniValueString = stringBuilder.ToString();
            if ((ret == 0) || string.IsNullOrEmpty(iniValueString)) { return false; }
            if (TypeDescriptor.GetConverter(typeof(T)) is not TypeConverter converter) { return false; }
            
            try
            {
                if (converter.ConvertFromString(iniValueString) is T iniValue)
                {
                    outputValue = iniValue;
                }
            }
            catch (NotSupportedException)
            {
                return false; 
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <returns>出力値</returns>
        public static T? GetValueOrDefault<T>(in string filePath,
                                             in string sectionName,
                                             in string keyName)
        {
            bool isSuccess = TryGetValueOrDafault(filePath, sectionName, keyName, default, out T? value);
            if (isSuccess && (value is not null)) { return value; }

            return default;
        }

        /// <summary>
        /// INIファイルからキーの値を取得します
        /// </summary>
        /// <typeparam name="T">データ取得する型</typeparam>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="defaultValue">初期値</param>
        /// <returns>出力値</returns>
        public static T? GetValueOrDefault<T>(in string filePath,
                                             in string sectionName,
                                             in string keyName,
                                             in T? defaultValue)
        {
            bool isSuccess = TryGetValueOrDafault(filePath, sectionName, keyName, defaultValue, out T? value);
            if (isSuccess && (value is not null)) { return value; }

            return defaultValue;
        }

        /// <summary>
        /// INIファイルにデータを書き込みます
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="sectionName">セクション名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="value">書き込む値</param>
        /// <returns>true : 書き込み成功 / false : 書き込み失敗</returns>
        public static bool SetValue(in string filePath, in string sectionName, in string keyName, in string value)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { return false; }
            if (!File.Exists(filePath))
            {
                if (!CreateFile(filePath)) { return false; }
            }

            int ret = NativeMethods.WritePrivateProfileString(sectionName, keyName, value, filePath);
            return (ret != 0);
        }

        /// <summary>
        /// ファイルがない場合は作成します
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>true : 作成成功 or 存在していた / false : 作成失敗 or 存在していない</returns>
        private static bool CreateFile(in string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { return false; }
            if (File.Exists(filePath)) { return true; }

            using FileStream fs = File.Create(filePath);
            if (fs is null) { return false; }

            return true;
        }
    }
}
