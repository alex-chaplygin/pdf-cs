# pdf-cs
Просмотр PDF файла на c#
## Стиль программирования
- Имена классов
> DeflateFilter
- Имена методов
> NextChar()
- Имена переменных
> lastChar
- Отступы в выражениях
> for (int i = 1; i < 5; i++)
- Один оператор на строку кода
>         if (lastChar == _false[i - 1])
>			  NextChar();
>         else
>                        throw new Exception("Ошибка Boolean");
- Открывающая скобка { на новой строке
> 	      while (lastChar == '%' || Parser.IsWhitespace(lastChar))
>	      {
>			if (lastChar == '%')
>			   	     SkipComment();
>			NextChar();
>		}
- Одно объявление переменной на строку кода
- Длинные строки разбиваются
- Одна пустая строка между методами и объявлениями полей класса
- Комментарии на отдельных строках
>	    // Случай true
- Не более одной пустой строки внутри метода
- Желательно, чтобы метод имел размер не более одного экрана
- Использовать переменные внутри строк
>	    int a = 1;
>	    string s = $"Строка {a}";  // Правильно
>	    string s = "Строка " + Convert.ToString(a);   // Неправильно
- Упрощение логических выражений
> if (flag == true) // Неправильно
> if (flag)               // Правильно
> if (flag2 == false) // Неправильно
> if (!flag2)               // Правильно
- Использовать var только если виден тип переменной
>	    var a = true;      // Правильно
>	    var b = 12;	       // Правильно
>	    var c = "string";  // Правильно
>	    var d = c + 1;     // Неправильно
- Указывать тип в цикле foreach
>	    foreach (char c in str)
- Для ввода-вывода, где нужно закрывать файл или уничтожать объект использовать using
>     using (StreamReader sr = new StreamReader(file))
>     {
>		sr.ReadLine();
>	}

## Настройка git

Для начала работы:

	git clone https://github.com/alex-chaplygin/pdf-cs

Настройка удалённого репозитория

	git remote set-url origin https://github.com/alex-chaplygin/pdf-cs


Чтобы проверить правильность параметров предыдущего пункта:

	git remote -v

Там должны быть строчки:

>origin  https://github.com/alex-chaplygin/pdf-cs (fetch)
>
>origin  https://github.com/alex-chaplygin/pdf-cs (push)
	

Настройка профиля:
	
	git config --global user.name "ВАШЕ ИМЯ НА GITHUB"
	git config --global user.email "ВАША ПОЧТА"
	
Настройка ядра git:
	
	git config --system core.autocrlf input
	git config core.repositoryformatversion 0
	git config core.filemode true
	git config core.bare false
	git config core.logallrefupdates true

***
## Порядок работы

0. Удаляем свою старую ветку

git checkout master

git branch -D iss<номер сделанной ветки>

1. Смотрим задание в разделе Issues

2. Синхронизировать удаленное хранилище и локальное

git pull origin master

3. Создаем ветку

git branch iss<номер>

git checkout iss<номер>

4. Работаем

5. Фиксация изменений

выполняем для каждого измененного файла

git add имя_файла

git commit -m "Краткое сообщение что было сделано"

6. Загрузить ветку на удаленный репозиторий

git push origin iss<номер>
