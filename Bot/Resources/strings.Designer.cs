﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bot.Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal strings() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Bot.Resources.strings", typeof(strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Я не пью.
        /// </summary>
        internal static string alcoMarkerNegative {
            get {
                return ResourceManager.GetString("alcoMarkerNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Я пью.
        /// </summary>
        internal static string alcoMarkerPositive {
            get {
                return ResourceManager.GetString("alcoMarkerPositive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 2130370689:AAFG7q1kKssieObubR8I1UcY4VxaWTs1xw4.
        /// </summary>
        internal static string botToken {
            get {
                return ResourceManager.GetString("botToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Не удалось перевести возраст в число :(.
        /// </summary>
        internal static string CantParseAgeError {
            get {
                return ResourceManager.GetString("CantParseAgeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Редактировать мою анкету.
        /// </summary>
        internal static string cardEditCommand {
            get {
                return ResourceManager.GetString("cardEditCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Сменить возраст.
        /// </summary>
        internal static string changeAgeCommand {
            get {
                return ResourceManager.GetString("changeAgeCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Сменить описание анкеты.
        /// </summary>
        internal static string changeDescriptionCommand {
            get {
                return ResourceManager.GetString("changeDescriptionCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Сменить имя.
        /// </summary>
        internal static string changeNameCommand {
            get {
                return ResourceManager.GetString("changeNameCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Нет, ещё подумаю.
        /// </summary>
        internal static string confirmationNegative {
            get {
                return ResourceManager.GetString("confirmationNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Да, подтверждаю.
        /// </summary>
        internal static string confirmationPositive {
            get {
                return ResourceManager.GetString("confirmationPositive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Я девушка.
        /// </summary>
        internal static string GenderFemaleConst {
            get {
                return ResourceManager.GetString("GenderFemaleConst", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Я парень.
        /// </summary>
        internal static string GenderMaleConst {
            get {
                return ResourceManager.GetString("GenderMaleConst", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Не указывать.
        /// </summary>
        internal static string GenderNeutralConst {
            get {
                return ResourceManager.GetString("GenderNeutralConst", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на поиск людей по указанному доходу.
        /// </summary>
        internal static string greedyModeDescription {
            get {
                return ResourceManager.GetString("greedyModeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Для начала нужно создать анкету....
        /// </summary>
        internal static string greetingHookRu {
            get {
                return ResourceManager.GetString("greetingHookRu", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Приветствую, .
        /// </summary>
        internal static string greetingRu1 {
            get {
                return ResourceManager.GetString("greetingRu1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Добро пожаловать, .
        /// </summary>
        internal static string greetingRu2 {
            get {
                return ResourceManager.GetString("greetingRu2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Здравствуйте, .
        /// </summary>
        internal static string greetingRu3 {
            get {
                return ResourceManager.GetString("greetingRu3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Рад видеть вас, .
        /// </summary>
        internal static string greetingRu4 {
            get {
                return ResourceManager.GetString("greetingRu4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Привет, .
        /// </summary>
        internal static string greetingRu5 {
            get {
                return ResourceManager.GetString("greetingRu5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на поиск людей без вредных привычек (курение, алкоголь).
        /// </summary>
        internal static string healthyModeDescription {
            get {
                return ResourceManager.GetString("healthyModeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на C:\Users\fpdcore\Source\Repos\fpdqwe\Besties\Bot\Resources\Images\.
        /// </summary>
        internal static string imagesPath {
            get {
                return ResourceManager.GetString("imagesPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Недопустимый возраст.
        /// </summary>
        internal static string InvalidAgeError {
            get {
                return ResourceManager.GetString("InvalidAgeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ответ не распознан. Пожалуйста, повторите попытку.
        /// </summary>
        internal static string InvalidAnswerError {
            get {
                return ResourceManager.GetString("InvalidAnswerError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Включить анкету.
        /// </summary>
        internal static string makeCardActiveCommand {
            get {
                return ResourceManager.GetString("makeCardActiveCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выключить анкету.
        /// </summary>
        internal static string makeCardInactiveCommand {
            get {
                return ResourceManager.GetString("makeCardInactiveCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Готово!.
        /// </summary>
        internal static string menuEndCommand {
            get {
                return ResourceManager.GetString("menuEndCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Итак, что вы хотите сделать?.
        /// </summary>
        internal static string menuText {
            get {
                return ResourceManager.GetString("menuText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на C:\Users\fpdcore\Source\Repos\fpdqwe\Besties\Bot\Resources\Regions.xml.
        /// </summary>
        internal static string regionsXmlPath {
            get {
                return ResourceManager.GetString("regionsXmlPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на C:\Users\fpdcore\Source\Repos\fpdqwe\Besties\Bot\Resources\.
        /// </summary>
        internal static string resourcesPath {
            get {
                return ResourceManager.GetString("resourcesPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Поиск.
        /// </summary>
        internal static string searchCommand {
            get {
                return ResourceManager.GetString("searchCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Параметры поиска.
        /// </summary>
        internal static string searchParamsCommand {
            get {
                return ResourceManager.GetString("searchParamsCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Показать изменённую анкету.
        /// </summary>
        internal static string showMyCardChangesCommand {
            get {
                return ResourceManager.GetString("showMyCardChangesCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Показать мою анкету.
        /// </summary>
        internal static string showMyCardCommand {
            get {
                return ResourceManager.GetString("showMyCardCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Я не курю.
        /// </summary>
        internal static string smokingMarkerNegative {
            get {
                return ResourceManager.GetString("smokingMarkerNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Я курю.
        /// </summary>
        internal static string smokingMarkerPositive {
            get {
                return ResourceManager.GetString("smokingMarkerPositive", resourceCulture);
            }
        }
    }
}
