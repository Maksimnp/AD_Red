# ADManager - Управление Active Directory 

ADManager — это приложение с графическим пользовательским интерфейсом, разработанное для управления пользователями и подразделениями (OU) в Active Directory. Оно позволяет выполнять поиск, создание, редактирование и удаление пользователей, а также просматривать список подразделений.

---

## Функциональные возможности

1. **Поиск пользователей**:
   - Поиск пользователей по имени (sAMAccountName).
   - Отображение полной информации о пользователе: логин, фамилия, имя, должность, email, номер телефона и другие атрибуты.

2. **Редактирование данных пользователей**:
   - Возможность обновления атрибутов пользователя (например, имя, фамилия, должность, email и т.д.).
   - Сохранение изменений в Active Directory.

3. **Создание новых пользователей**:
   - Создание новых учетных записей пользователей с указанием всех необходимых атрибутов.
   - Настройка пароля и добавление пользователя в определенное подразделение (OU).

4. **Просмотр подразделений (OU)**:
   - Получение списка всех подразделений (OU) в домене.
   - Просмотр пользователей, принадлежащих к выбранному подразделению.

5. **Интуитивный интерфейс**:
   - Современный дизайн
   - Автоматическое обновление интерфейса при изменении размеров окна.

---

## Требования

Для работы приложения необходимы следующие компоненты:

- **Операционная система**: Windows (тестировалось на Windows 10/11).
- **.NET Framework**: .NET 6.0 или выше.
- **Доступ к Active Directory**: Учетные данные администратора домена для подключения к Active Directory.

